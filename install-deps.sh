#!/bin/sh

install_linux() {
  sudo apt-get update
  sudo apt-get install gforth libffi-dev libltdl7 libsoil-dev yodl
  if [ `uname -m`$M32 = x86_64-m32 ]; then
    sudo apt-get --fix-missing install gcc-multilib libltdl7:i386
  fi
}

install_osx() {
  brew tap zchee/homebrew-zsh
  brew update > /dev/null
  brew install yodl
  brew install gforth
}

install_${TRAVIS_OS_NAME:-linux}
./install-swig.sh
