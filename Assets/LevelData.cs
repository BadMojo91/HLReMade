using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class TileData {
    public int x,y;
    public string tileImage;
    public byte tileType;
    public bool obj;
    public char tile;
    public TileData() {
        x = 0;
        y = 0;
        tileImage = "";
        tileType = 0;
    }
}

public class LevelData{
    public int levelNumber;
    public string levelName;
    public int gridSizeX, gridSizeY;
    public int pStartX, pStartY;
    public TileData[,] tileData;
    public static LevelData[] defaultLevels;
    public LevelData() {
        tileData = new TileData[1, 1];
        levelName = null;
        levelNumber = 0;
        gridSizeX = 1;
        gridSizeY = 1;
    }
    public LevelData(TileData[,] td, string name, int num, int x, int y, int pX, int pY) {
        tileData = td;
        levelName = name;
        levelNumber = num;
        gridSizeX = x;
        gridSizeY = y;
        pStartX = pX;
        pStartY = pY;
    }

    public static void ImportHLLevelData() {
        defaultLevels = new LevelData[100];
        //Load LEVELS.TXT from resources
        TextAsset heartLevels = Resources.Load("LEVELS") as TextAsset;

        //Replace new lines with char 'l'
        string hString = heartLevels.text;
        hString = hString.Replace("\r\n", "l");
        //hString = hString.Replace("\n", "n");
        Debug.Log(hString);
        //Add string to char array
        char[] hlChar = hString.ToCharArray();

        //Variables
        List<char> ld = new List<char>();
        
        bool readingLevel = false;
        bool skip = false;
        int levelCount = 0;
        int cX=0, cY=0;
        int pX=0, pY=0;
        TileData[,] td;
        //Read level data
        foreach(char c in hlChar) {
            //Start of level
            if(c == '{') {
                readingLevel = true;
                ld = new List<char>();
                ld.Clear();
                cX = 20;

            }

            else if(readingLevel) {
                
                //End of level
                if(c == '}') {
                    //cY--;
                    td = new TileData[20, cY];
                    int lCount = 0;

                    for(int y = cY - 1; y >= 0; y--) {
                        for(int x = 0; x < 20; x++) {
                            td[x, y] = new TileData();
                            // Debug.Log(ld.Count + " : " + lCount);
                            //if(lCount >= ld.Count)
                            //    lCount = ld.Count - 1;

                            td[x, y].tile = ld[lCount];//Set the tile char in tile data

                            if(td[x, y].tile == '.') {
                                td[x, y].tileType = 2;
                                td[x, y].tileImage = "Grass";
                            }
                            else if(td[x, y].tile == '%') {
                                td[x, y].tileType = 1;
                                td[x, y].tileImage = "Metal";
                            }
                            else if(td[x, y].tile == '&') {
                                td[x, y].tileType = 5;
                                td[x, y].tileImage = "";
                                td[x, y].obj = true;
                            }
                            else if(td[x, y].tile == '$') {
                                td[x, y].tileType = 6;
                                td[x, y].tileImage = "";
                                td[x, y].obj = true;
                            }
                            else if(td[x, y].tile == '@') {
                                td[x, y].tileType = 7;
                                td[x, y].tileImage = "";
                                td[x, y].obj = true;
                            }
                            else if(td[x, y].tile == '*') {
                                td[x, y].tileType = 8;
                                td[x, y].obj = true;
                                pX = x;
                                pY = y;
                            }
                            else if(td[x, y].tile == ' ') {
                                td[x, y].tileType = 0;
                                td[x, y].tileImage = "";
                            }
                            else {
                                td[x, y].tileType = 1;
                                td[x, y].tileImage = "Metal";
                            }
                            lCount++;
                        }
                    }
                    SetDefaultLevelData(levelCount, new LevelData(td, "Level: " + levelCount, levelCount, 20, cY, pX, pY));
                    readingLevel = false;
                    skip = false;
                    cX = 0;
                    cY = 0;
                    pX = 0;
                    pY = 0;
                    levelCount++;//Increment ready for next level
                }

                else if(c == 'l') {
                    if(!skip) { //Skip first new line
                        skip = true;
                    }
                    else {

                        for(; cX < 20; cX++) {
                            if(cX >= 19)
                                break;
                            else {
                                ld.Add(' ');
                            }
                        }
                        cX = 0;
                        cY++; //Set height of grid
                    }
                  

                }
                else {
                    ld.Add(c);
                    cX++;
                }
                if(cX >= 20)
                    cX = 0;

            }

           
            //The Level
            

        }
        //yield return new WaitForEndOfFrame();
    }
    
    public static void SetDefaultLevelData(int levelNum, LevelData ld) {
        defaultLevels[levelNum] = ld;
    }
}
