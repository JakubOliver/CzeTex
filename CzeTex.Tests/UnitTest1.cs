namespace CzeTex.Tests;

using Xunit;
using CzeTex;

using System;

public class UnitTest1
{
    [Fact]
    public void CanReadFile()
    {
        Assert.Equal("Hello, World!", new Files(new string[] {"examples", "example1.txt"}).LoadFile()[0]);
    }

    [Fact]
    public void FileDoesNotExist(){
        Assert.Throws<System.IO.FileNotFoundException>(() => new Files(new string[] {"examples", "neexistuje.txt"}));
    }
}
