#!/usr/bin/env python3

import argparse
import json
import re
from pathlib import Path


def _fold_block(lines: list[str]) -> str:
    paragraphs: list[str] = []
    current: list[str] = []

    for line in lines:
        stripped = line.strip()
        if not stripped:
            if current:
                paragraphs.append(" ".join(current))
                current = []
            continue
        current.append(stripped)

    if current:
        paragraphs.append(" ".join(current))

    return "\n\n".join(paragraphs)


def parse_build_yaml(path: Path) -> dict:
    data: dict[str, object] = {}
    artifacts: list[str] = []
    lines = path.read_text(encoding="utf-8").splitlines()
    index = 0

    while index < len(lines):
        line = lines[index].rstrip()

        if not line or line == "---":
            index += 1
            continue

        scalar_match = re.match(r'^([A-Za-z][A-Za-z0-9_]*)\s*:\s*"([^"]*)"\s*$', line)
        if scalar_match:
            data[scalar_match.group(1)] = scalar_match.group(2)
            index += 1
            continue

        block_match = re.match(r"^([A-Za-z][A-Za-z0-9_]*)\s*:\s*>\s*$", line)
        if block_match:
            key = block_match.group(1)
            block_lines: list[str] = []
            index += 1

            while index < len(lines):
                block_line = lines[index]
                if block_line.startswith("  ") or not block_line.strip():
                    block_lines.append(block_line[2:] if block_line.startswith("  ") else "")
                    index += 1
                    continue
                break

            data[key] = _fold_block(block_lines)
            continue

        if re.match(r"^artifacts\s*:\s*$", line):
            index += 1
            while index < len(lines):
                artifact_line = lines[index].rstrip()
                artifact_match = re.match(r'^\s*-\s*"([^"]+)"\s*$', artifact_line)
                if artifact_match:
                    artifacts.append(artifact_match.group(1))
                    index += 1
                    continue
                break
            data["artifacts"] = artifacts
            continue

        raise ValueError(f"Unsupported build.yaml line: {line}")

    return data


def parse_version(value: str) -> tuple[int, ...]:
    return tuple(int(part) for part in value.split("."))


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--build-yaml", required=True)
    parser.add_argument("--manifest", required=True)
    parser.add_argument("--source-url", required=True)
    parser.add_argument("--checksum", required=True)
    parser.add_argument("--timestamp", required=True)
    args = parser.parse_args()

    metadata = parse_build_yaml(Path(args.build_yaml))
    manifest_path = Path(args.manifest)

    if manifest_path.exists():
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    else:
        manifest = []

    guid = metadata["guid"]
    plugin = next((item for item in manifest if item.get("guid") == guid), None)

    if plugin is None:
        plugin = {}
        manifest.append(plugin)

    plugin.update(
        {
            "guid": metadata["guid"],
            "name": metadata["name"],
            "description": metadata["description"],
            "overview": metadata["overview"],
            "owner": metadata["owner"],
            "category": metadata["category"],
        }
    )

    version_entry = {
        "version": metadata["version"],
        "changelog": metadata["changelog"],
        "targetAbi": metadata["targetAbi"],
        "sourceUrl": args.source_url,
        "checksum": args.checksum,
        "timestamp": args.timestamp,
    }

    versions = [
        item
        for item in plugin.get("versions", [])
        if item.get("version") != metadata["version"]
    ]
    versions.append(version_entry)
    versions.sort(key=lambda item: parse_version(item["version"]), reverse=True)
    plugin["versions"] = versions

    manifest.sort(key=lambda item: item["name"].lower())
    manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
