using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Noobie.Sanguosha.Editor
{
    public class CreateBMFont : MonoBehaviour
    {
        [MenuItem("Tool/CreateBMFont")]
        static void CreateFont()
        {
            Object obj = Selection.activeObject;
            string fntPath = AssetDatabase.GetAssetPath(obj);
            string fontName = Path.GetFileNameWithoutExtension(fntPath);
            if (fntPath.IndexOf(".fnt", StringComparison.Ordinal) == -1)
            {
                // 不是字体文件 
                Debug.LogError("The Selected Object Is Not A .fnt file!");
                return;
            }

            string customFontPath = fntPath.Replace(".fnt", ".fontsettings");
            Font customFont;
            if (!File.Exists(customFontPath))
            {
                customFont = new Font();
                AssetDatabase.CreateAsset(customFont, customFontPath);
            }
            else
            {
                customFont = AssetDatabase.LoadAssetAtPath<Font>(customFontPath);
            }

            string pngPath = fntPath.Replace(".fnt", ".png");
            if (!File.Exists(pngPath))
            {
                Debug.LogError("未找到同名图片");
                return;
            }
            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(pngPath);

            string matPath = fntPath.Replace(".fnt", ".mat");
            Material mat;
            if (!File.Exists(matPath))
            {
                mat = new Material(Shader.Find("GUI/Text Shader"))
                {
                    mainTexture = texture,
                    name = fontName
                };
                AssetDatabase.CreateAsset(mat, matPath);
            }
            else
            {
                mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                mat.mainTexture = texture;
            }

            customFont.material = mat;

            StreamReader reader = new StreamReader(new FileStream(fntPath, FileMode.Open));

            List<CharacterInfo> charList = new List<CharacterInfo>();

            Regex reg = new Regex(@"char id=(?<id>\d+)\s+x=(?<x>\d+)\s+y=(?<y>\d+)\s+width=(?<width>\d+)\s+height=(?<height>\d+)\s+xoffset=(?<xoffset>(-|\d)+)\s+yoffset=(?<yoffset>(-|\d)+)\s+xadvance=(?<xadvance>\d+)\s+", RegexOptions.Compiled);
            string line = reader.ReadLine();
            int lineHeight = 65;
            int texWidth = 512;
            int texHeight = 512;

            while (line != null)
            {
                if (line.IndexOf("char id=", StringComparison.Ordinal) != -1)
                {
                    Match match = reg.Match(line);
                    if (match != Match.Empty)
                    {
                        var id = Convert.ToInt32(match.Groups["id"].Value);
                        var x = Convert.ToInt32(match.Groups["x"].Value);
                        var y = Convert.ToInt32(match.Groups["y"].Value);
                        var width = Convert.ToInt32(match.Groups["width"].Value);
                        var height = Convert.ToInt32(match.Groups["height"].Value);
                        var xOffset = Convert.ToInt32(match.Groups["xoffset"].Value);
                        var yOffset = Convert.ToInt32(match.Groups["yoffset"].Value);
                        var xAdvance = Convert.ToInt32(match.Groups["xadvance"].Value);
                        Debug.Log("ID" + id);

                        CharacterInfo info = new CharacterInfo
                        {
                            index = id
                        };
                        float uvx = 1f * x / texWidth;
                        float uvy = 1 - (1f * y / texHeight);
                        float uvw = 1f * width / texWidth;
                        float uvh = -1f * height / texHeight;

                        info.uvBottomLeft = new Vector2(uvx, uvy);
                        info.uvBottomRight = new Vector2(uvx + uvw, uvy);
                        info.uvTopLeft = new Vector2(uvx, uvy + uvh);
                        info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);

                        info.minX = -xOffset;
                        info.minY = lineHeight / 2 - yOffset; // 纵向的偏移取统一的高度
                        info.glyphWidth = width;
                        info.glyphHeight = -height;
                        info.advance = xAdvance;

                        charList.Add(info);
                    }
                }
                else if (line.IndexOf("scaleW=", StringComparison.Ordinal) != -1)
                {
                    Regex reg2 = new Regex(@"common lineHeight=(?<lineHeight>\d+)\s+.*scaleW=(?<scaleW>\d+)\s+scaleH=(?<scaleH>\d+)", RegexOptions.Compiled);
                    Match match = reg2.Match(line);
                    if (match != Match.Empty)
                    {
                        lineHeight = Convert.ToInt32(match.Groups["lineHeight"].Value);
                        texWidth = Convert.ToInt32(match.Groups["scaleW"].Value);
                        texHeight = Convert.ToInt32(match.Groups["scaleH"].Value);
                    }
                }
                line = reader.ReadLine();
            }

            customFont.characterInfo = charList.ToArray();
            EditorUtility.SetDirty(customFont);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(customFont);
        }
    }
}
