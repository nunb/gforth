\ substitute stuff

require string.fs

wordlist AConstant macros-wordlist

: macro: ( addr u -- ) Create here 0 , $! DOES> $@ ;

: replaces ( addr1 len1 addr2 len2 -- )
    \G create a macro with name @var{addr2 len2} and content @var{addr1 len1}.
    \G if the macro already exists, just change the content.
    2dup macros-wordlist search-wordlist
    IF  nip nip >body $!
    ELSE
	get-current >r macros-wordlist set-current
	['] macro: execute-parsing
	r> set-current
    THEN ;

: .% ( -- ) '%' emit ;
: .substitute ( addr1 len1 -- n / ior )
    \G substitute all macros in text @var{addr1 len1} and print the
    \G result.  @var{n} is the number of substitutions or, if
    \G negative, a throwable @var{ior}.
    0 >r
    BEGIN  dup  WHILE  '%' $split
	    2swap type dup IF
		over c@ '%' = IF
		    .% 1 /string
		ELSE
		    '%' $split 2swap dup 0= IF
			2drop .%
		    ELSE
			2over drop 1- c@ '%' = IF
			    2dup macros-wordlist search-wordlist IF
				nip nip -rot
				2>r execute type 2r> r> 1+ >r
			    ELSE
				.% type .%
			    THEN
			ELSE
			    .% type
			THEN
		    THEN
		THEN
	    ELSE
		over 1- c@ '%' = IF  .%  THEN
	    THEN
    REPEAT 2drop r> ;

: $substitute ( addr1 len1 -- addr2 len2 n/ior )
    \G substitute all macros in text @var{addr1 len1}.  @var{n} is the
    \G number of substitutions, if negative, it's a throwable @{ior},
    \G @var{addr2 len2} the result.
    ['] .substitute $tmp rot ;

: substitute ( addr1 len1 addr2 len2 -- addr2 len3 n/ior )
    \G substitute all macros in text @var{addr1 len1}, and copy the
    \G result to @var{addr2 len2}.  @var{n} is the number of
    \G substitutions or, if negative, a throwable @var{ior},
    \G @var{addr2 len3} the result.
    tmp$ @ >r tmp$ off
    2>r $substitute over r@ u<= -78 swap select -rot
    2r> rot umin 2dup 2>r move 2r> rot tmp$ $off r> tmp$ ! ;

: unescape ( addr1 u1 dest -- dest u2 )
    \G double all delimiters in @var{addr1 u1}, so that substitute
    \G will result in the original text.  Note that the buffer
    \G @var{dest} does not have a size, as in worst case, it will need
    \G just twice as many characters as @var{u1}. @{dest u2} is the
    \G resulting string.
    dp @ >r dup >r dp !
    bounds ?DO
	I c@ dup '%' = IF  dup c,  THEN  c,
    LOOP  r> here over -  r> dp ! ;