using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class InspectorList<T>
{
    [SerializeField]
    public T[] values;

    public InspectorList()
    {
        values = new T[0];
    }

    public int Count
    {
        get { return values.Length; }
    }

    public T Get(int index)
    {
        return values[index];
    }

    public void Add(T newEntry)
    {
        Resize(values.Length + 1);
        values[values.Length - 1] = newEntry;
    }

    public void Remove(T entry)
    {
        T[] newValues = new T[values.Length - 1];

        int diff = 0;

        //Copy over values
        for (int i = 0; i < newValues.Length; i++)
        {
            if (values[i].Equals(entry))
            {
                diff = 1;
                continue;
            }

            newValues[i] = values[i + diff];
        }

        values = newValues;
    }

    public void Resize(int newSize)
    {
        T[] newValues = new T[newSize];

        //Copy over values
        for (int i = 0; i < newValues.Length; i++)
        {
            if (i >= values.Length)
                break;

            newValues[i] = values[i];
        }

        values = newValues;
    }

}

