/* support functions for vmgen example

  Copyright (C) 2001 Free Software Foundation, Inc.

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

#include <stdio.h>

typedef void *Label; 

#ifdef USE_CASTS
typedef long Cell;
Typedef Cell Inst;

/* type change macros; these are specific to the types you use, so you
   have to change this part */
#define vm_Cell2i(_cell,x) ((x)=(long)(_cell))
#define vm_i2Cell(_cell,x) ((x)=(Cell)(_cell))
#define vm_Cell2target(_cell,x) ((x)=(Inst *)(_cell))
#define vm_target2Cell(_cell,x) ((x)=(Cell)(_cell))
#define vm_Cell2a(_cell,x) ((x)=(char *)(_cell))
#define vm_a2Cell(_cell,x) ((x)=(Cell)(_cell))
#else
typedef union Cell {
  long i;
  union Cell *target;
  Label inst;
  char *a;
} Cell, Inst;

#define vm_Cell2i(_cell,_x)	((_x)=(_cell).i)
#define vm_i2Cell(_x,_cell)	((_cell).i=(_x))	
#define vm_Cell2target(_cell,_x) ((_x)=(_cell).target)
#define vm_target2Cell(_x,_cell) ((_cell).target=(_x))	
#define vm_Cell2a(_cell,_x)	((_x)=(_cell).a)
#define vm_a2Cell(_x,_cell)	((_cell).a=(_x))	
#endif

#define VM_IS_INST(_inst, n) ((_inst).inst == vm_prim[n])

extern Label *vm_prim;
extern int locals;
extern struct Peeptable_entry **peeptable;
extern int vm_debug;
extern FILE *yyin;
extern int yylineno;
extern char *program_name;
extern FILE *vm_out;
extern Inst *vmcodep;
extern Inst *last_compiled;
extern Inst *vmcode_end;

/* generic vmgen support functions (e.g., wrappers) */
void gen_inst(Inst **vmcodepp, Label i);
void init_peeptable(void);
void vm_disassemble(Inst *ip, Inst *endp, Label prim[]);
void vm_count_block(Inst *ip);
struct block_count *block_insert(Inst *ip);
void vm_print_profile(FILE *file);

/* mini type-specific support functions */
void genarg_i(Inst **vmcodepp, long i);
void printarg_i(long i);
void genarg_target(Inst **vmcodepp, Inst *target);
void printarg_target(Inst *target);
void printarg_a(char *a);
void printarg_Cell(Cell i);

/* engine functions (type not fixed) */
long engine(Inst *ip0, Cell *sp, char *fp);
long engine_debug(Inst *ip0, Cell *sp, char *fp);

/* other generic functions */
int yyparse(void);

/* mini-specific functions */
void insert_func(char *name, Inst *start, int locals, int nonparams);
Inst *func_addr(char *name);
long func_calladjust(char *name);
void insert_local(char *name);
long var_offset(char *name);
void gen_main_end(void);

/* stack pointer change for a function with n nonparams */
#define adjust(n)  ((n) * -sizeof(Cell))
