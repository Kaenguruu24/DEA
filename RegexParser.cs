using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace Automata;

public class RegexParser
{
    private string _pattern;
    private int _cursor;
    private bool _hasNextChar;
    private char _nextChar;

    private List<string> Metachars;
    private List<string> Sets;

    private List<char> OpenedSets;

    public Godot.Collections.Dictionary LoadJsonFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var data = Json.ParseString(json);

        return data.AsGodotDictionary();
    }

    public RegexParser(string pattern)
    {
        _pattern = pattern;
        _cursor = 0;
        _hasNextChar = true;

        Metachars = new List<string>();
        Sets = new List<string>();
        OpenedSets = new List<char>();

        var data = LoadJsonFile("D:/Projects/Godot/DEA/regex_grammar.json");
        foreach (Godot.Collections.Dictionary metachar in data["metacharacters"].AsGodotArray())
        { Metachars.Add(metachar["character"].ToString()); }

        foreach (Godot.Collections.Dictionary metachar in data["sets"].AsGodotArray())
        { Metachars.Add(metachar["identifier"].ToString()); }
    }

    public void Parse()
    {
        while (_hasNextChar)
        {
            _nextChar = _pattern[_cursor];
            if (Metachars.Contains(_nextChar.ToString()))
            { ParseMetachar(); }
            else if (Sets.Contains(_nextChar.ToString()))
            { ParseSet(); }
            else
            { ParseLiteral(); }
            NextChar();
        }

        if (OpenedSets.Count > 0)
        { GD.PrintErr("Error: Unclosed set"); }
    }

    private void ParseMetachar()
    {
        GD.Print("Metachar: " + _pattern[_cursor]);
        if (new List<char> {'[', '(', '{'}.Contains(_nextChar))
        { OpenedSets.Add(_nextChar); }
        else if (new List<char> {']', ')', '}'}.Contains(_nextChar))
        { OpenedSets.RemoveAt(OpenedSets.Count - 1); }
    }
    private void ParseSet()
    { GD.Print("Set: " + _pattern[_cursor]); }
    private void ParseLiteral()
    { GD.Print("Literal: " + _pattern[_cursor]); }

    private void NextChar()
    {
        _cursor++;
        _hasNextChar = _cursor < _pattern.Length;
    }
}