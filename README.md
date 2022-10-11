# BROT

[![build](https://github.com/kievzenit/binary-rotate/actions/workflows/ci-build.yml/badge.svg?branch=master&event=push)](https://github.com/kievzenit/binary-rotate/actions/workflows/ci-build.yml)

BROT - binary rotator, a CLI project for "ciphering" any data.

It uses binary data rotation algorithm invented by me)

## Installation


```bash
git clone https://github.com/kievzenit/binary-rotate
```

## Usage

1. Build
```bash
dotnet build
```
2. Run:

```bash
.\brot -d 17 -n test.txt text.txt directory
```

- -d: rotation count or "degrees"
- -n: negative rotation
- test.txt, directory and so on: it's files that must be ciphered

## Contributing
Pull requests are welcome!) For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
