/* rint replacement

  Copyright (C) 2002 Free Software Foundation, Inc.

  This file is part of Gforth.

  Gforth is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111, USA.
*/

#ifdef i386
#define X (1024.*1024.*1024.*1024.*1024.*1024.*16.)
#else /* !defined(386) */
#define X (1024.*1024.*1024.*1024.*1024.*8.)
#endif /* !defined(386) */
double rint(double r)
{
  if (r<0.0)
    return (r+X)-X;
  else
    return (r-X)+X;
}
