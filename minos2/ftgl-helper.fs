\ freetype GL example

\ freetype stuff

require unix/freetype-gllib.fs

also freetype-gl
also opengl

512 Value atlas#

atlas# dup 1 texture_atlas_new Value atlas

tex: atlas-tex
atlas-tex current-tex atlas texture_atlas_t-id !

\ : atlas-tex  atlas texture_atlas_t-id l@ dup to current-tex
\     GL_TEXTURE_2D swap glBindTexture ;

\ render font into vertex buffers

2 sfloats buffer: penxy
Variable color $FFC0A0FF color !

: xy, { glyph -- }
    penxy sf@ penxy sfloat+ sf@ { f: xp f: yp }
    glyph texture_glyph_t-offset_x l@ s>f
    glyph texture_glyph_t-offset_y l@ s>f { f: xo f: yo }
    glyph texture_glyph_t-width  @ s>f
    glyph texture_glyph_t-height @ s>f { f: w f: h }    
    xp xo f+  yp yo f- { f: x0 f: y0 }
    x0 w f+ y0 h f+ { f: x1 f: y1 }
    glyph texture_glyph_t-s0 sf@ { f: s0 }
    glyph texture_glyph_t-t0 sf@ { f: t0 }
    glyph texture_glyph_t-s1 sf@ { f: s1 }
    glyph texture_glyph_t-t1 sf@ { f: t1 }
    >v
    x0 y0 >xy n> color @ rgba>c s0 t0 >st v+
    x1 y0 >xy n> color @ rgba>c s1 t0 >st v+
    x0 y1 >xy n> color @ rgba>c s0 t1 >st v+
    x1 y1 >xy n> color @ rgba>c s1 t1 >st v+
    v>
    xp glyph texture_glyph_t-advance_x sf@ f+ penxy sf!
    yp glyph texture_glyph_t-advance_y sf@ f+ penxy sfloat+ sf! ;

: glyph+xy ( glyph -- )
    i>off  xy,  2 quad ;

: all-glyphs ( -- )
    i>off >v
    0e 0e >xy n> color @ rgba>c 0e 0e >st v+
    512e 0e >xy n> color @ rgba>c 1e 0e >st v+
    0e 512e >xy n> color @ rgba>c 0e 1e >st v+
    512e 512e >xy n> color @ rgba>c 1e 1e >st v+
    v> 2 quad ;

0 Value font

: xchar+xy ( prev-xchar xchar -- xchar )
    tuck font swap texture_font_get_glyph >r
    dup 0> IF  r@ swap texture_glyph_get_kerning
	penxy sf@ f+ penxy sf!
    ELSE  drop  THEN
    r> glyph+xy ;

: render-string ( addr u -- )
    0 -rot  bounds ?DO
	I xc@+ swap >r xchar+xy r>
    I - +LOOP  drop ;

: xchar@xy ( fw fd fh prev-xchar xchar -- xchar )
    tuck font swap texture_font_get_glyph >r
    dup 0> IF  r@ swap texture_glyph_get_kerning  f+
    ELSE  drop  THEN
    { f: fd f: fh }
    r@ texture_glyph_t-advance_x sf@ f+
    r@ texture_glyph_t-offset_y l@ s>f
    r> texture_glyph_t-height l@ s>f
    fover f- fnegate fd fmin fswap fh fmax ;

: layout-string ( addr u -- fw fh fd ) \ depth is ow far it goes down
    0 -rot  0e 0e 0e  bounds ?DO
	I xc@+ swap >r xchar@xy r>
    I - +LOOP  drop ;

: load-glyph$ ( addr u -- ) font -rot texture_font_load_glyphs drop ;

: load-ascii ( -- )
    "#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~" load-glyph$ ;

program init

: <render ( -- )
    program glUseProgram
    1-bias set-color+
    .01e 100e 100e >ap
    atlas-tex v0 i0 ;

: render> ( -- )  GL_TRIANGLES draw-elements
    ( Coloradd 0e fdup fdup fdup glUniform4f ) ;

previous previous