using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class Utils
{
    public static void WriteQuaternionsIntoFile(string fileName, string filePath, Quaternion[,] quaternions, int rowNum, int columnNum)
    {
        string file = filePath + "/" + fileName;

        if (File.Exists(file))
        {
            Debug.Log(fileName + " already exists.");
            
        }

        var sr = File.CreateText(fileName);

        for (int i = 0; i < rowNum; i ++)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < columnNum; j++)
            {
                Quaternion q = quaternions[i, j];
                sb.Append(q.x).Append(" ").Append(q.y).Append(" ").Append(q.z).Append(" ").Append(q.w).Append("|");
            }
            if (sb.Length > 0) // remove last "|"
                sb.Remove(sb.Length - 1, 1);
            sr.WriteLine(sb.ToString());
            
        }
        sr.Close();
    }

    public static void WriteVector3IntoFile(string fileName, string filePath, Vector3[,] vector3s, int rowNum, int columnNum)
    {
        string file = filePath + "/" + fileName;

        if (File.Exists(file))
        {
            Debug.Log(fileName + " already exists.");
      
        }

        var sr = File.CreateText(fileName);

        for (int i = 0; i < rowNum; i++)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < columnNum; j++)
            {
                Vector3 v = vector3s[i, j];
                sb.Append(v.x).Append(" ").Append(v.y).Append(" ").Append(v.z).Append("|");
            }
            if (sb.Length > 0) // remove last "|"
                sb.Remove(sb.Length - 1, 1);
            sr.WriteLine(sb.ToString());

        }
        sr.Close();
    }

    public static Quaternion[,] ReadQuaternionsFromFile(string fileName, string filePath, int rowNum, int columnNum)
    {
        Quaternion[,] result = new Quaternion[rowNum, columnNum];
        string file = filePath + "/" + fileName;
        StreamReader sr = new StreamReader(file);

        int i = 0;
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            string[] quaternions = line.Split('|');

            for (int j = 0; j < quaternions.Length; j++)
            {
                string[] values = quaternions[j].Split(' ');
                if (values.Length != 4)
                    throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
                result[i,j] = new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
            }
            i++;
            // Do Something with the input. 
        }

        sr.Close();

        return result;
    }


    public static Vector3[,] ReadVector3sFromFile(string fileName, string filePath, int rowNum, int columnNum)
    {

        Vector3[,] result = new Vector3[rowNum, columnNum];
       
        string file = filePath + "/" + fileName;
        StreamReader sr = new StreamReader(file);

        int i = 0;
        while (!sr.EndOfStream)
        {
            
            string line = sr.ReadLine();
            string[] vectors= line.Split('|');

            for (int j = 0; j < vectors.Length; j++)
            {
                string[] values =vectors[j].Split(' ');
                if (values.Length != 3)
                    throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
                result[i,j] = new Vector3 (float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
            }
            i++;
            // Do Something with the input. 
        }

        sr.Close();

        return result;
    }



}
