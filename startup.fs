\ startup file

\ Copyright (C) 1995 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program; if not, write to the Free Software
\ Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

warnings off

\ include float.fs
\ include search-order.fs
include glocals.fs
include environ.fs
\ include toolsext.fs
include wordinfo.fs
include vt100.fs
\ include colorize.fs
include see.fs
include bufio.fs
include debug.fs
include history.fs
\ include doskey.fs
include vt100key.fs
require debugging.fs
require assert.fs
require blocks.fs

0 Value $?
: sh  '# parse cr system  to $? ;

\ define the environmental queries for all the loaded wordsets
\ since the blocks wordset is loaded in a single file, its queries
\ are defined there
\ queries for other things than presence of a wordset are answered
\ in environ.fs
get-current environment-wordlist set-current
true constant double
true constant double-ext
true constant exception
true constant exception-ext
true constant facility
\ !! facility-ext
true constant file
true constant file-ext
true constant floating
true constant floating-ext
true constant locals
true constant locals-ext
true constant memory-alloc
true constant memory-alloc-ext
true constant tools
\ !! tools-ext
true constant search-order
true constant search-order-ext
true constant string
true constant string-ext
set-current



warnings on
