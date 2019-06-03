# JM Tools ~ Build System

Build system for Unity plus itch.io integration through butler

## Prerequisites

To upload builds to itch.io, you will need to have butler installed and setup.

1. Download butler from itch.io: https://fasterthanlime.itch.io/butler
2. Extract to a directory on your computer
3. On Windows, add butler to your path (TODO other OS's)
4. TODO setup your account with butler

## Troubleshooting

`Win32Exception: The system cannot find the file specified.`

This can occur when clicking "Upload" if the butler path is incorrect.

## Known Issues

Zip operation (ZipFile class) may not work on non-Windows sytems (untested).
