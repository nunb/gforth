\ png library

e? os-type s" linux-android" string-prefix? [IF]
    s" libpng.so" also c-lib open-path-lib drop previous
[THEN]

Vocabulary pnglib
get-current also pnglib definitions

c-library png
    e? os-type s" linux-android" string-prefix? [IF]
	s" png" add-lib
	\c #include <zlib.h>
	\c #include "../../../../libpng/pngconf.h"
	\c #include "../../../../libpng/png.h"
    [ELSE]
	[IFDEF] linux
	    s" png12" add-lib
	    \c #include <zlib.h>
	    \c #include <libpng12/pngconf.h>
	    \c #include <libpng12/png.h>
	[THEN]
    [THEN]
    include unix/pnglib.fs

end-c-library

previous set-current