using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Runtime;
using UnityEngine;

[CreateAssetMenu(menuName = "Create DataFileConverter", fileName = "DataFileConverter", order = 0)]
public class DataFileConverter : ScriptableObject
{
   
   public string filePath;
   [Button]
   public void CreateFile()
   {
      StreamReader reader = new StreamReader(filePath);
      bool fileEmpty = false;
      string[] savedTime = null;
      while (!fileEmpty)
      {
         string nextLine = reader.ReadLine();
         if (nextLine == null)
         {
            fileEmpty = true;
            break;
         }

         string[] splitLine = nextLine.Split(",");
         
         if (splitLine[^2] == "1")
         {
            savedTime = splitLine;
         }

         if (splitLine[^2] == "2")
         {
            
         }
      }
      reader.Close();

      string path = @$"{Environment.CurrentDirectory}\Assets/TestData\NwqDiwl.txt";
      Debug.Log("printing to :" + path);
      StreamWriter sw = File.CreateText(path);
      sw.WriteLine();
      sw.Flush();
   }
}
