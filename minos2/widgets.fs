\ MINOS2 widget basis

\ A MINOS2 widget is composed of drawable elements, boxes and actors.
\ to make things easier, neither drawable elements nor boxes need an actor.

require gl-helper.fs
ctx 0= [IF] window-init [THEN]

require ftgl-helper.fs
require mini-oof2.fs

get-current
also [IFDEF] android android [THEN]
also opengl

vocabulary minos  also minos definitions

0 Value layer \ drawing layer

object class
    field: caller-w
    method clicked
    method keyed
    method inside?
    method focus
    method defocus
    method show
    method hide
    method get
    method set
    method show-you
end-class actor

object class
    field: next-w
    field: parent-w
    field: x
    field: y
    field: w
    field: h \ above baseline
    field: d \ below baseline
    method draw ( -- )
    method hglue ( -- typ sub add )
    method dglue ( -- typ sub add )
    method vglue ( -- typ sub add )
    method hglue@ ( -- typ sub add ) \ cached variant
    method dglue@ ( -- typ sub add ) \ cached variant
    method vglue@ ( -- typ sub add ) \ cached variant
    method xywh ( -- x0 y0 w h )
    method xywhd ( -- x y w h d )
    method resize ( x y w h d -- )
    method !size \ set your own size
end-class widget

:noname x @ y @ h @ - w @ h @ d @ + ; widget to xywh
:noname x @ y @ w @ h @ d @ ; widget to xywhd
' noop widget to !size
:noname w @ 0 0 ; widget to hglue
:noname h @ 0 0 ; widget to vglue
:noname d @ 0 0 ; widget to dglue
:noname d ! h ! w ! y ! x ! ; widget to resize
' hglue widget to hglue@
' vglue widget to vglue@
' dglue widget to dglue@

tex: style-tex \ 8 x 8 subimages, each sized 128x128
style-tex 1024 dup rgba-newtex

\ tile widget

widget class
    field: frame#
    field: frame-color
end-class tile

8 Value style-w#
8 Value style-h#

: #>st ( x y frame -- ) \ using frame#
    style-w# /mod
    s>f f+ style-w# fm/ fswap
    s>f f+ style-h# fm/ fswap >st ;

: draw-rectangle { f: x1 f: y1 f: x2 f: y2 -- }
    i? >v
    x1 y2 >xy frame-color @ rgba>c n> 0e 1e frame# @ #>st v+
    x2 y2 >xy frame-color @ rgba>c n> 1e 1e frame# @ #>st v+
    x2 y1 >xy frame-color @ rgba>c n> 1e 0e frame# @ #>st v+
    x1 y1 >xy frame-color @ rgba>c n> 0e 0e frame# @ #>st v+
    v> dup i, dup 1+ i, dup 2 + i, dup i, dup 2 + i, 3 + i, ;
: tile-draw ( -- ) layer 1 <> ?EXIT
    xywh { x y w h }
    x s>f y s>f x w + s>f y h + s>f
    draw-rectangle GL_TRIANGLES draw-elements ;

' tile-draw tile is draw

\ frame widget

tile class
    field: border
end-class frame

Create button-st  0e sf, 0.25e sf, 0.75e sf, 1e sf,
DOES>  swap sfloats + sf@ ;
: button-border ( n -- gray )  dup 2/ xor ;
: >border ( x b i w -- r ) >r
    button-border >r
    r@ 1 and 0= IF drop 0       THEN
    r> 2 and    IF negate r@ +  THEN  + s>f  rdrop ;

: frame-draw ( -- ) layer 1 <> ?EXIT
    frame# @ frame-color @ border @ xywh { f c b x y w h }
    i>off >v
    4 0 DO
	4 0 DO
	    x b I w >border  y b J h >border >xy
	    c rgba>c  n>
	    I button-st J button-st f #>st v+
	LOOP
    LOOP
    v>
    9 0  DO
	4 quad  1 I 3 mod 2 = - i-off +!
    LOOP
; ' frame-draw frame to draw

\ text widget

widget class
    field: text-string
    field: text-font
    field: text-color
end-class text

Variable glyphs$

: text-draw ( -- )
    layer 2 = IF
	text-font @ to font text-string $@ glyphs$ $+!
	EXIT  THEN
    layer 3 = IF
	x @ s>f penxy sf!  y @ s>f penxy sfloat+ sf!
	text-font @ to font  text-color @ color !
	text-string $@ render-string THEN ;
: text-!size ( -- )
    text-string $@ layout-string
    f>s d ! f>s h ! f>s w ! ;
' text-draw text to draw
' text-!size text to !size

\ draw wrapper

: <draw0 ( -- )  0 to layer
    -1e 1e >apxy
    .01e 100e 100e >ap
    0.01e 0.02e 0.15e 1.0e glClearColor
    Ambient 1 ambient% glUniform1fv ;
: draw0> ( -- ) clear v0 i0 ;

: <draw1 ( -- )  1 to layer
    z-bias set-color+
    program glUseProgram  style-tex ;
: draw1> ( -- )  GL_TRIANGLES draw-elements v0 i0 ;

: <draw2 ( -- )  2 to layer s" " glyphs$ $! ;
: draw2> ( -- )  glyphs$ $@ load-glyph$ ;
: <draw3 ( -- )  3 to layer
    <render ;
: draw3> ( -- )  render> v0 i0 ;

Variable style-i#

: load-style ( addr u -- n )  style-tex
    style-i# @ 8 /mod 128 * >r 128 * r> 2swap load-subtex 2drop
    style-i# @ 1 style-i# +! ;
: style: load-style Create , DOES> @ frame# ! ;

"button.png" style: button1
"button2.png" style: button2
"button3.png" style: button3

\ glues

widget class
    3 cells +field hglue-c
    3 cells +field dglue-c
    3 cells +field vglue-c
end-class glue

: @+ ( addr -- u addr' )  dup >r @ r> cell+ ;
: !- ( addr -- u addr' )  dup >r ! r> cell- ;
: glue@ ( addr -- t s a )  @+ @+ @ ;
: glue! ( t s a addr -- )  2 cells + !- !- ! ;
:noname hglue-c glue@ ; dup glue to hglue@ glue to hglue
:noname dglue-c glue@ ; dup glue to dglue@ glue to dglue
:noname vglue-c glue@ ; dup glue to vglue@ glue to vglue

\ boxes

glue class
    field: child-w
    field: act
    method resized
    method map
end-class box

: do-childs { xt -- .. }
    child-w @ >o
    BEGIN  xt execute  next-w @ o>  dup  WHILE  >o  REPEAT
    drop ;

:noname ( -- )
    ['] !size do-childs
    hglue hglue-c glue!
    dglue dglue-c glue!
    vglue vglue-c glue! ; box to !size

:noname ( -- ) ['] draw do-childs ; box to draw

: +child ( o -- ) child-w @ over >o next-w ! o> child-w ! ;
: +childs ( o1 .. on n -- ) 0 +DO  +child  LOOP ;

\ glue arithmetics

box class end-class hbox \ horizontal alignment
box class
    field: baseline \ minimun skip per line
end-class vbox \ vertical alignment
box class end-class zbox \ overlay alignment

: 0glue ( -- t s a ) 0 0 0 ;
: 1glue ( -- t s a ) 0 0 [ -1 1 rshift ]L ;

: g3>2 ( t s a -- min a ) over + >r - r> ;

: glue+ { t1 s1 a1 t2 s2 a2 -- t3 s3 a3 }
    t1 t2 + s1 s2 + a1 a2 + ;
: glue* { t1 s1 a1 t2 s2 a2 -- t3 s3 a3 }
    t1 t2 max
    t1 s1 - t2 s2 - max over - 0 max
    t1 a1 + t2 a2 + min 2 pick - 0 max ;
: baseglue ( -- b 0 max )
    baseline @ 0 [ -1 1 rshift ]L ;
: glue-drop ( t s a -- )  2drop drop ;

: hglue+ 0glue [: hglue@ glue+ ;] do-childs ;
: dglue+ 0glue [: glue-drop dglue@ ;] do-childs ; \ last dglue
: vglue+ 0glue 0glue [: vglue@ glue+ baseglue glue* glue+ dglue@ ;] do-childs
    glue-drop ;

: hglue* 1glue [: hglue@ glue* ;] do-childs ;
: dglue* 1glue [: dglue@ glue* ;] do-childs ;
: vglue* 1glue [: vglue@ glue* ;] do-childs ;

' hglue+ hbox is hglue
' dglue* hbox is dglue
' vglue* hbox is vglue

' hglue* vbox is hglue
' dglue+ vbox is dglue
' vglue+ vbox is vglue

' hglue* zbox is hglue
' dglue* zbox is dglue
' vglue* zbox is vglue

\ add glues up for hboxes

: hglue-step { gp ga rd rg rx -- gp ga rd' rg' rx' }
    gp ga  rx x !
    hglue@ g3>2 { xmin xa }
    rg xa + gp ga */ rd - dup rd + rg xa +
    rot xmin +  dup x @ - w ! ;

: hbox-resize { x h d -- x h d } x y @ w @ h d resize  x h d ;

\ add glues up for vboxes

: vglue-step { gp ga rd rg ry td sd ad -- gp ga rd' rg' ry' td' sd' ad' }
    gp ga baseglue
    vglue@ td sd ad glue+ glue* g3>2 { ymin ya }
    rg ya + gp ga */ rd - dup rd + rg ya +
    rot ymin +  dup ry !  dglue@ ;

previous previous previous set-current