PlayNow.StarTar
===============

A .Net Standard for reading and writing GNU tar archives. The library also has
some limited support for older versions of tar archives.

Supported type flags: `0`/`\0` (regular file), `5` (directory) and `L` (long
name). Other flags can be read, but cannot automatically be handled.

File ownership is currently unhandled.