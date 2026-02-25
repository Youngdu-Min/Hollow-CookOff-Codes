using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hamster.ZG.Type;

//List<float>에 대한 정의
[Type(typeof(List<float>), new string[] { "List<float>", "list<float>" })]
public class FloatListType : IType
{
    public object DefaultValue => new List<float>();

    public object Read(string value)
    {
        var values = ReadUtil.GetBracketValueToArray(value);

        List<float> floats = new List<float>();
        for (int i = 0; i < values.Length; i++)
        {
            floats.Add(float.Parse(values[i]));
        }
        return floats;
    }

    public string Write(object value)
    {
        List<float> floats = (List<float>)value;

        string join = string.Join(",", floats);
        return $"[{join}]";
    }
}

//List<int>에 대한 정의
[Type(typeof(List<int>), new string[] { "List<int>", "list<int>" })]
public class IntListType : IType
{
    public object DefaultValue => new List<int>();

    public object Read(string value)
    {
        var values = ReadUtil.GetBracketValueToArray(value);

        List<int> floats = new List<int>();
        for (int i = 0; i < values.Length; i++)
        {
            floats.Add(int.Parse(values[i]));
        }
        return floats;
    }

    public string Write(object value)
    {
        List<int> ints = (List<int>)value;

        string join = string.Join(",", ints);
        return $"[{join}]";
    }
}

//bool에 대한 정의
[Type(typeof(bool), new string[] { "bool", "Bool" })]
public class BoolType : IType
{
    public object DefaultValue => false;

    public object Read(string value)
    {
        var y = bool.TryParse(value, out bool x);

        if (y == false)
        {
            return DefaultValue;
        }
        return x;
    }

    public string Write(object value)
    {
        return value.ToString();
    }
}


//벡터2에 대한 정의
[Type(typeof(Vector2), new string[] { "vector2", "Vector2" })]
public class Vector2Type : IType
{
    public object DefaultValue => Vector2.zero;

    public object Read(string value)
    {
        var values = ReadUtil.GetBracketValueToArray(value);
        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        return new Vector2(x, y);
    }
    public string Write(object value)
    {
        Vector2 vec2 = (Vector2)value;
        return $"[{vec2.x},{vec2.y}]";
    }
}
