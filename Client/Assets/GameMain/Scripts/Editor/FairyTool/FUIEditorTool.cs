using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FairyGUI;
using FairyGUI.Utils;
using UnityEditor;
using UnityEngine;

public class FUIEditorTool : MonoBehaviour
{
    [MenuItem("Game Framework/FairyGUI/导出UIBinder")]
    private static void ExportUIBinder()
    {
        var sourcePath = Application.dataPath + @"/GameMain/Scripts/HotFix/GameLogic/Generate/";
        var targetPath = Application.dataPath + @"/GameMain/Scripts/HotFix/GameLogic/Generate/UIBinder.cs";
        var csFiles = Directory.GetFiles(sourcePath, "*Binder.cs", SearchOption.AllDirectories);
        var usingArr = new string[csFiles.Length];
        var bindArr = new string[csFiles.Length];
        var usingStr = string.Empty;
        var bindStr = string.Empty;
        for (var i = 0; i < csFiles.Length; i++)
        {
            var str = csFiles[i].Replace(sourcePath, "");
            str = str.Replace(".cs", "");
            var arr = str.Split('\\');
            if (arr.Length < 2) continue;
            usingArr[i] = $"using GameLogic.{arr[0]};";
            usingStr += $"using GameLogic.{arr[0]};\n";
            bindArr[i] = $"{arr[1]}.BindAll();";
            bindStr += $"\t\t\t{arr[1]}.BindAll();\n";
        }

        // 开始写入文件
        Debug.Log("=====开始写文件=====");
        var codeContent = File.ReadAllText(targetPath, Encoding.UTF8);
        var content = codeContent;
        var startStr = "/******using脚本自动生成部分起始******/";
        var endStr = "/******using脚本自动生成部分结束******/";
        var startIndex = content.IndexOf(startStr);
        var endIndex = content.IndexOf(endStr);
        if (startIndex == -1 || endIndex == -1)
        {
            Debug.LogError("未找到起始或结束标记");
            return;
        }

        var oldValue = content.Substring(startIndex, endIndex - startIndex);
        oldValue += endStr;
        var newValue = startStr + "\n" + usingStr + endStr;
        content = content.Replace(oldValue, newValue);


        startStr = "/******脚本自动生成部分起始******/";
        endStr = "/******脚本自动生成部分结束******/";
        startIndex = content.IndexOf(startStr);
        endIndex = content.IndexOf(endStr);
        if (startIndex == -1 || endIndex == -1)
        {
            Debug.LogError("未找到起始或结束标记");
            return;
        }

        oldValue = content.Substring(startIndex, endIndex - startIndex);
        oldValue += endStr;
        newValue = startStr + "\n" + bindStr + "\t\t\t" + endStr;
        content = content.Replace(oldValue, newValue);
        File.WriteAllText(targetPath, content, Encoding.UTF8);
        AssetDatabase.Refresh();
        Debug.Log("=====写文件完成=====");
    }

    public enum FUIReasonEnum
    {
        UseOtherPackageAssets, // 引用了其他的包          
        UseGray, // 使用了置灰
        UseColorFilter, // 使用了滤镜
        ErrorBlendMode, // 使用了错误的混合模式
        UseGraphMask, // 使用了自定义遮罩
        UseOverflowTypeHidden, // 使用了隐藏的溢出处理
        AssetsMissing // 资源丢失
    }

    public static string[] FUI_REASONS = new[]
    {
        "使用了其他非公共包内资源",
        "基本属性中设置了置灰",
        "效果属性中设置了滤镜",
        "效果属性中设置了错误的BlendMode",
        "基本组件中设置了自定义遮罩",
        "基本组件中设置了错误的溢出处理",
        "使用了已经丢失的资源"
    };

    private static string[] BASIC_UIS = new[]
    {
        "Basics",
    };


    private static UIPackageAnalysisInfo _curPackageAnalysisInfo;
    private static UIPackage _curAnalysisPackage;
    private static List<UICheckComponentInfo> _needCheckComponentList;
    private static string m_curCheckGObjectName;

    public static List<UIPackageAnalysisInfo> packageAnalysisInfoList;
    public static TransitionContent _transitionContent;


    [MenuItem("Game Framework/FairyGUI/导出PackageAnalysis ")]
    public static void UIPackageCheck()
    {
        UIPackage.RemoveAllPackages();
        var assetGUIDs = AssetDatabase.FindAssets("_fui", new[] { "Assets/AssetRaw/UI" });
        foreach (var assetGUID in assetGUIDs)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            assetPath = assetPath.Replace("_fui.bytes", "");
            UIPackage.AddPackage(assetPath);
        }

        packageAnalysisInfoList = new List<UIPackageAnalysisInfo>();
        _needCheckComponentList = new List<UICheckComponentInfo>();
        _transitionContent = new TransitionContent();
        var loadPackageList = UIPackage.GetPackages();
        var count = loadPackageList.Count;
        for (var i = 0; i < count; i++)
        {
            AnalysisPackage(loadPackageList[i]);
            EditorUtility.DisplayProgressBar("FairyGUI配置分析", "正在分析：" + loadPackageList[i].name, (i + 1) / count);
        }

        EditorUtility.ClearProgressBar();
        OutputAnalysisFile();
        UIPackage.RemoveAllPackages();
    }

    private static void OutputAnalysisFile()
    {
        //输出分析文件
        var stringBuilder = new StringBuilder();
        foreach (var analysisInfo in packageAnalysisInfoList)
        {
            stringBuilder.AppendLine(analysisInfo.packageItemName);
            stringBuilder.AppendLine("    ├─── Atlas Infos:");
            foreach (var kv in analysisInfo.atlasAnalysisInfoDict)
            {
                var atlasName = kv.Value.id.PadRight(10, ' ');
                var atlasWidth = kv.Value.width.ToString().PadRight(4, ' ');
                var atlasHeight = kv.Value.height.ToString().PadRight(4, ' ');
                var atlasFillingRate = $"{kv.Value.fillingRate:P}";
                stringBuilder.AppendLine(
                    $"    │   ├─── {atlasName} Size: {atlasWidth} * {atlasHeight} FillingRate: {atlasFillingRate}");
            }

            stringBuilder.AppendLine("    ├─── Use Basic Package:");
            foreach (var name in analysisInfo.basicPackageUseSet)
            {
                stringBuilder.AppendLine($"    │   ├─── {name}");
            }

            stringBuilder.AppendLine("    ├─── Need Check:");
            foreach (var needCheckItem in analysisInfo.uiCheckComponentInfoList)
            {
                stringBuilder.AppendLine(needCheckItem.ToString());
            }

            stringBuilder.AppendLine("    └───────");
        }

        var filePath = Application.dataPath + "/UIPackageAnalysis.txt";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var fs = new FileStream(filePath, FileMode.Create);
        var streamWriter = new StreamWriter(fs);
        streamWriter.Write(stringBuilder.ToString());
        streamWriter.Flush();
        streamWriter.Close();
        fs.Close();

        //输出多语言文件
        filePath = Application.dataPath + "/TransitionContent.json";
        fs = new FileStream(filePath, FileMode.Create);
        streamWriter = new StreamWriter(fs);
        streamWriter.Write(JsonUtility.ToJson(_transitionContent));
        streamWriter.Flush();
        streamWriter.Close();
        fs.Close();

        var output = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var pathF = $"{output}/UILangConfig.csv";

        fs = new FileStream(pathF, FileMode.Create, FileAccess.Write);
        streamWriter = new StreamWriter(fs, Encoding.UTF8);

        streamWriter.WriteLine("id,text,name,componentName,packageName");
        var len = _transitionContent.content.Count;
        for (var i = 0; i < len; i++)
        {
            var trans = _transitionContent.content[i];
            streamWriter.WriteLine(
                $"{trans.id},{trans.value},{trans.name},{trans.componentName},{trans.packageName}");
        }

        EditorUtility.ClearProgressBar();
        streamWriter.Close();
        fs.Close();
        //DataTableToCsv();
        AssetDatabase.Refresh();
    }

    //转Excel
    private static void DataTableToCsv()
    {
        var table = new DataTable { TableName = "UILang" };
        table.Columns.Add(new DataColumn { ColumnName = "id" });
        table.Columns.Add(new DataColumn { ColumnName = "text" });
        table.Columns.Add(new DataColumn { ColumnName = "name" });
        table.Columns.Add(new DataColumn { ColumnName = "componentName" });
        table.Columns.Add(new DataColumn { ColumnName = "packageName" });
        table.AcceptChanges();
        var len = _transitionContent.content.Count;
        for (var i = 0; i < len; i++)
        {
            var trans = _transitionContent.content[i];
            var dr = table.NewRow();
            dr[0] = trans.id;
            dr[1] = trans.value;
            dr[2] = trans.name;
            dr[3] = trans.componentName;
            dr[4] = trans.packageName;
            table.Rows.Add(dr);
            table.AcceptChanges();
        }

        Console.WriteLine("转Excel...");
        var title = "";
        var output = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var pathF = $"{output}/UILangExcel.xlsx";
        var fs = new FileStream(pathF, FileMode.OpenOrCreate);

        var sw = new StreamWriter(new BufferedStream(fs), Encoding.Default);
        for (var i = 0; i < table.Columns.Count; i++)
        {
            title += table.Columns[i].ColumnName + "\t"; //栏位：自动跳到下一单元格
        }

        title = title.Substring(0, title.Length - 1) + "\n";

        sw.Write(title);

        foreach (DataRow row in table.Rows)
        {
            var line = "";
            for (var i = 0; i < table.Columns.Count; i++)
            {
                line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格
            }

            line = line.Substring(0, line.Length - 1) + "\n";
            sw.Write(line);
        }

        sw.Close();
        fs.Close();
        Console.WriteLine("转Excel完成...");
    }

    private static void AnalysisPackage(UIPackage uiPackage)
    {
        _curPackageAnalysisInfo = new UIPackageAnalysisInfo(uiPackage.name);
        _curAnalysisPackage = uiPackage;
        var packageItemList = uiPackage.GetItems();
        foreach (var packageItem in packageItemList)
        {
            switch (packageItem.type)
            {
                case PackageItemType.Atlas:
                    AnalysisItemAtlas(packageItem);
                    break;
                case PackageItemType.Image:
                    AnalysisItemImage(packageItem);
                    break;
                case PackageItemType.Component:
                    AnalysisItemComponent(packageItem);
                    TranslateComponent(packageItem);
                    break;
                case PackageItemType.Font:
                    break;
                case PackageItemType.MovieClip:
                    AnalysisItemMovieClip(packageItem);
                    break;
                default:
                    break;
            }
        }

        _curPackageAnalysisInfo.DoAnalysis();
    }

    private static void AnalysisItemMovieClip(PackageItem packageItem)
    {
        packageItem.Load();
        _curPackageAnalysisInfo.AddMovieClipItem(packageItem);
    }

    private static void AnalysisItemAtlas(PackageItem packageItem)
    {
        var texture = packageItem.owner.GetItemAsset(packageItem) as NTexture;
        _curPackageAnalysisInfo.AddAtlasItem(packageItem);
    }

    private static void AnalysisItemImage(PackageItem packageItem)
    {
        var texture = packageItem.owner.GetItemAsset(packageItem) as NTexture;
        _curPackageAnalysisInfo.AddImagePackageItem(packageItem);
    }

    private static void TranslateComponent(PackageItem item)
    {
        string ids, names;
        ByteBuffer buffer = item.rawData;

        buffer.Seek(0, 2);

        int childCount = buffer.ReadShort();
        for (int i = 0; i < childCount; i++)
        {
            int dataLen = buffer.ReadShort();
            int curPos = buffer.position;

            buffer.Seek(curPos, 0);
            ObjectType type = (ObjectType)buffer.ReadByte();
            if (type == ObjectType.Component ||
                type == ObjectType.Text ||
                type == ObjectType.RichText ||
                type == ObjectType.InputText ||
                type == ObjectType.List ||
                type == ObjectType.Label ||
                type == ObjectType.Button ||
                type == ObjectType.ComboBox)
            {
                buffer.Skip(4);
                ids = item.owner.id + item.id + "-" + buffer.ReadS();
                names = buffer.ReadS();
                //Debug.Log("ids: " + ids);
                //Debug.Log("names: " + names);

                if (type == ObjectType.Component)
                {
                    if (buffer.Seek(curPos, 6))
                        type = (ObjectType)buffer.ReadByte();
                }

                buffer.Seek(curPos, 1);

                //-tips
                //Debug.Log("-tips: " + buffer.ReadS());

                buffer.Seek(curPos, 2);

                int gearCnt = buffer.ReadShort();
                for (int j = 0; j < gearCnt; j++)
                {
                    int nextPos = buffer.ReadShort();
                    nextPos += buffer.position;

                    if (buffer.ReadByte() == 6) //gearText
                    {
                        buffer.Skip(2); //controller
                        int valueCnt = buffer.ReadShort();
                        for (int k = 0; k < valueCnt; k++)
                        {
                            string page = buffer.ReadS();
                            if (page != null)
                            {
                                //-texts_
                                string texts_ = buffer.ReadS();
                                if (!string.IsNullOrEmpty(texts_))
                                {
                                    AddTranslateContent(item.owner.name, item.name, ids + "-texts_" + k, names,
                                        texts_);
                                    //Debug.Log("-texts_" + j + ": " + texts_);
                                }
                            }
                        }

                        //-texts_def
                        if (buffer.ReadBool())
                        {
                            string texts_def = buffer.ReadS();
                            if (!string.IsNullOrEmpty(texts_def))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids + "-texts_def", names,
                                    texts_def);
                                //Debug.Log("-texts_def: " + texts_def);
                            }
                        }
                    }

                    buffer.position = nextPos;
                }

                switch (type)
                {
                    case ObjectType.Text:
                    case ObjectType.RichText:
                    {
                        //text
                        buffer.Seek(curPos, 6);
                        string text = buffer.ReadS();
                        if (!string.IsNullOrEmpty(text))
                        {
                            AddTranslateContent(item.owner.name, item.name, ids, names, text);
                            //Debug.Log("-text: " + text);
                        }

                        break;
                    }
                    case ObjectType.InputText:
                    {
                        //text
                        buffer.Seek(curPos, 6);
                        string text = buffer.ReadS();
                        if (!string.IsNullOrEmpty(text))
                        {
                            AddTranslateContent(item.owner.name, item.name, ids, names, text);
                            //Debug.Log("-text: " + text);
                        }

                        //-prompt
                        buffer.Seek(curPos, 4);
                        string prompt = GetPromptContent(buffer.ReadS());
                        if (!string.IsNullOrEmpty(prompt))
                        {
                            AddTranslateContent(item.owner.name, item.name, ids + "-prompt", names, prompt);
                            //Debug.Log("-prompt: " + prompt);
                        }

                        break;
                    }

                    case ObjectType.List:
                    {
                        buffer.Seek(curPos, 8);
                        buffer.Skip(2);
                        int itemCount = buffer.ReadShort();
                        for (int j = 0; j < itemCount; j++)
                        {
                            int nextPos = buffer.ReadShort();
                            nextPos += buffer.position;

                            buffer.Skip(2); //url
                            //"-" + j
                            string _j = buffer.ReadS();
                            if (!string.IsNullOrEmpty(_j))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids + "-" + j, names, _j);
                                //Debug.Log("-" + j + ": " + _j);
                            }

                            //"-" + j + "-0"
                            string _j_ = buffer.ReadS();
                            if (!string.IsNullOrEmpty(_j_))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids + "-" + j + "-0", names, _j_);
                                //Debug.Log("_" + j + "_0: " + _j_);
                            }

                            buffer.position = nextPos;
                        }

                        break;
                    }

                    case ObjectType.Label:
                    {
                        if (buffer.Seek(curPos, 6) && (ObjectType)buffer.ReadByte() == type)
                        {
                            string text = buffer.ReadS();
                            if (!string.IsNullOrEmpty(text))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids, names, text);
                                //Debug.Log("-text: " + text);
                            }

                            buffer.Skip(2);
                            if (buffer.ReadBool())
                                buffer.Skip(4);

                            buffer.Skip(4);
                            if (buffer.ReadBool())
                            {
                                //-prompt
                                string prompt = GetPromptContent(buffer.ReadS());
                                if (!string.IsNullOrEmpty(prompt))
                                {
                                    AddTranslateContent(item.owner.name, item.name, ids + "-prompt", names, prompt);
                                    //Debug.Log("-prompt: " + prompt);
                                }
                            }
                        }

                        break;
                    }

                    case ObjectType.Button:
                    {
                        if (buffer.Seek(curPos, 6) && (ObjectType)buffer.ReadByte() == type)
                        {
                            string text = buffer.ReadS();
                            if (!string.IsNullOrEmpty(text))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids, names, text);
                                //Debug.Log("-text: " + text);
                            }

                            //-0
                            string _0 = buffer.ReadS();
                            if (!string.IsNullOrEmpty(_0))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids + "-0", names, _0);
                                //Debug.Log("-0: " + _0);
                            }
                        }

                        break;
                    }

                    case ObjectType.ComboBox:
                    {
                        if (buffer.Seek(curPos, 6) && (ObjectType)buffer.ReadByte() == type)
                        {
                            int itemCount = buffer.ReadShort();
                            for (int j = 0; j < itemCount; j++)
                            {
                                int nextPos = buffer.ReadShort();
                                nextPos += buffer.position;

                                string _j = buffer.ReadS();
                                if (!string.IsNullOrEmpty(_j))
                                {
                                    AddTranslateContent(item.owner.name, item.name, ids + "-" + j, names, _j);
                                    //Debug.Log("-j: " + _j);
                                }

                                buffer.position = nextPos;
                            }

                            string text = buffer.ReadS();
                            if (!string.IsNullOrEmpty(text))
                            {
                                AddTranslateContent(item.owner.name, item.name, ids, names, text);
                                //Debug.Log("text: " + text);
                            }
                        }

                        break;
                    }
                }
            }

            buffer.position = curPos + dataLen;
        }
    }

    private static string GetPromptContent(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        //是否含中文
        var pattern = "[\u4e00-\u9fbb]";
        var containChinese = Regex.IsMatch(str, pattern);
        if (containChinese) return str;
        //是否全部字母数字组成
        var isDigitOrNumber = Regex.IsMatch(str, @"(?i)^[0-9a-z]+$");
        if (!isDigitOrNumber)
        {
            return str;
        }

        //是否全部小写
        pattern = (str.Trim()).Replace(" ", "");
        var allLower = (pattern == pattern.ToLowerInvariant());
        if (allLower) return string.Empty;
        return str;
    }


    private static void AddTranslateContent(string packageName, string componentName, string id, string name,
        string str, ObjectType fairyObjectType = ObjectType.Component)
    {
        //是否含中文
        var containChinese = Regex.IsMatch(str, "[\u4e00-\u9fbb]");
        if (!containChinese) return;
        var content = new TransitionValue
        {
            packageName = packageName,
            componentName = componentName,
            id = id,
            name = name,
            value = str
        };
        _transitionContent.content.Add(content);
    }

    [Serializable]
    public class TransitionValue
    {
        public string packageName;
        public string componentName;
        public string id;
        public string name;
        public string value;
    }

    [Serializable]
    public class TransitionContent
    {
        public List<TransitionValue> content = new List<TransitionValue>();
    }

    private static void AnalysisItemComponent(PackageItem packageItem)
    {
        var buffer = packageItem.owner.GetItemAsset(packageItem) as ByteBuffer;
        buffer.Seek(0, 0);
        buffer.ReadInt();
        buffer.ReadInt();

        if (buffer.ReadBool())
        {
            buffer.ReadInt();
            buffer.ReadInt();
            buffer.ReadInt();
            buffer.ReadInt();
        }

        if (buffer.ReadBool())
        {
            buffer.ReadFloat();
            buffer.ReadFloat();
        }

        if (buffer.ReadBool())
        {
            buffer.ReadInt();
            buffer.ReadInt();
            buffer.ReadInt();
            buffer.ReadInt();
        }

        var overflow = (OverflowType)buffer.ReadByte();
        if (overflow == OverflowType.Scroll)
        {
            var savedPos = buffer.position;
            buffer.Seek(0, 7);
            // 使用了scrollPanel
            buffer.position = savedPos;
        }
        else
        {
            if (overflow == OverflowType.Hidden)
            {
                // 使用了隐藏
                Debug.Log("AnalysisItemComponent ========== 使用了隐藏");
            }
        }

        if (buffer.ReadBool())
        {
            buffer.ReadInt();
            buffer.ReadInt();
        }

        // 初始化控制器组件
        buffer.Seek(0, 1);
        int controllerCount = buffer.ReadShort();
        for (var i = 0; i < controllerCount; i++)
        {
            int nextPos = buffer.ReadShort();
            nextPos += buffer.position;
            buffer.position = nextPos;
        }

        buffer.Seek(0, 2);

        int childCount = buffer.ReadShort();
        for (var i = 0; i < childCount; i++)
        {
            _needCheckComponentList.Clear();
            var dataLen = buffer.ReadShort();
            var curPos = buffer.position;

            buffer.Seek(curPos, 0);
            ObjectType type = (ObjectType)buffer.ReadByte();
            var src = buffer.ReadS();
            var pkgId = buffer.ReadS();

            PackageItem pi = null;
            if (src != null)
            {
                var pkg = pkgId != null ? UIPackage.GetById(pkgId) : packageItem.owner;

                if (pkg == null)
                {
                    _needCheckComponentList.Add(new UICheckComponentInfo(packageItem, pkg,
                        FUIReasonEnum.AssetsMissing));
                }
                else
                {
                    if (pkg != _curAnalysisPackage)
                    {
                        if (Array.IndexOf(BASIC_UIS, pkg.name) >= 0)
                        {
                            if (!_curPackageAnalysisInfo.basicPackageUseSet.Contains(pkg.name))
                            {
                                _curPackageAnalysisInfo.basicPackageUseSet.Add(pkg.name);
                            }
                        }
                        else
                        {
                            _needCheckComponentList.Add(new UICheckComponentInfo(packageItem, pkg));
                        }
                    }
                }

                pi = pkg?.GetItem(src);
            }

            AnalysisGObjectInfo(packageItem, buffer, curPos);
            if (_needCheckComponentList.Count > 0)
            {
                foreach (var needCheckChild in _needCheckComponentList)
                {
                    needCheckChild.needCheckChildName = m_curCheckGObjectName;
                    _curPackageAnalysisInfo.uiCheckComponentInfoList.Add(needCheckChild);
                }

                _needCheckComponentList.Clear();
            }

            buffer.position = curPos + dataLen;
        }

        buffer.Seek(0, 4);
        buffer.Skip(2); //customData
        buffer.ReadBool();
        int maskId = buffer.ReadShort();
        if (maskId != -1)
        {
            _curPackageAnalysisInfo.uiCheckComponentInfoList.Add(
                new UICheckComponentInfo(packageItem, FUIReasonEnum.UseGraphMask));
        }
    }

    private static void AnalysisGObjectInfo(PackageItem packageItem, ByteBuffer buffer, int beginPos)
    {
        buffer.Seek(beginPos, 0);
        buffer.Skip(5);
        m_curCheckGObjectName = buffer.ReadS();

        // SetXY(f1, f2);
        buffer.ReadInt();
        buffer.ReadInt();

        if (buffer.ReadBool())
        {
            buffer.ReadInt();
            buffer.ReadInt();
            // SetSize(initWidth, initHeight, true);
        }

        if (buffer.ReadBool())
        {
            buffer.ReadInt();
            buffer.ReadInt();
            buffer.ReadInt();
            buffer.ReadInt();
            // 最大最小宽高
        }

        if (buffer.ReadBool())
        {
            buffer.ReadFloat();
            buffer.ReadFloat();
            // SetScale(f1, f2);
        }

        if (buffer.ReadBool())
        {
            buffer.ReadFloat();
            buffer.ReadFloat();
            // this.skew = new Vector2(f1, f2);
        }

        if (buffer.ReadBool())
        {
            buffer.ReadFloat();
            buffer.ReadFloat();
            buffer.ReadBool();
            // SetPivot(f1, f2, buffer.ReadBool());
        }

        buffer.ReadFloat();
        // if (f1 != 1)
        // this.alpha = f1;
        // alpha

        buffer.ReadFloat();
        // if (f1 != 0)
        //    this.rotation = f1;
        // rotation
        if (!buffer.ReadBool())
        {
        }

        // this.visible = false;
        if (!buffer.ReadBool())
        {
        }

        //this.touchable = false;
        if (buffer.ReadBool())
        {
            _needCheckComponentList.Add(new UICheckComponentInfo(packageItem, FUIReasonEnum.UseGray));
        }
        // this.grayed = true;

        var blendMode = (BlendMode)buffer.ReadByte();
        if (blendMode != BlendMode.Normal)
        {
            _needCheckComponentList.Add(
                new UICheckComponentInfo(packageItem, FUIReasonEnum.ErrorBlendMode));
        }

        int filter = buffer.ReadByte();
        if (filter != 1) return;
        var cf = new ColorFilter();
        cf.AdjustBrightness(buffer.ReadFloat());
        cf.AdjustContrast(buffer.ReadFloat());
        cf.AdjustSaturation(buffer.ReadFloat());
        cf.AdjustHue(buffer.ReadFloat());
        _needCheckComponentList.Add(new UICheckComponentInfo(packageItem, FUIReasonEnum.UseColorFilter));
    }
}

public class UIPackageAnalysisInfo
{
    public readonly string packageItemName;
    public readonly Dictionary<NTexture, AtlasAnalysisInfo> atlasAnalysisInfoDict;

    public readonly List<UICheckComponentInfo> uiCheckComponentInfoList;
    public readonly HashSet<string> basicPackageUseSet;

    private readonly List<PackageItem> _atlasItemList;
    private readonly List<PackageItem> _movieClipItemList;
    private readonly Dictionary<NTexture, List<PackageItem>> _imageItemDict;


    public UIPackageAnalysisInfo(string name)
    {
        packageItemName = name;

        atlasAnalysisInfoDict = new Dictionary<NTexture, AtlasAnalysisInfo>();
        uiCheckComponentInfoList = new List<UICheckComponentInfo>();
        basicPackageUseSet = new HashSet<string>();

        _atlasItemList = new List<PackageItem>();
        _movieClipItemList = new List<PackageItem>();
        _imageItemDict = new Dictionary<NTexture, List<PackageItem>>();
    }

    public void AddImagePackageItem(PackageItem packageItem)
    {
        if (!_imageItemDict.TryGetValue(packageItem.texture.root, out var spriteItemList))
        {
            spriteItemList = new List<PackageItem>();
            _imageItemDict.Add(packageItem.texture.root, spriteItemList);
        }

        spriteItemList.Add(packageItem);
    }

    public void AddAtlasItem(PackageItem packageItem)
    {
        _atlasItemList.Add(packageItem);
    }

    public void AddMovieClipItem(PackageItem packageItem)
    {
        _movieClipItemList.Add(packageItem);
    }

    public void AddAtlasAnalysisInfo(AtlasAnalysisInfo info)
    {
    }

    public void DoAnalysis()
    {
        // Debug.Log("Cur Package Name  " + packageItemName);
        for (var i = 0; i < _atlasItemList.Count; i++)
        {
            var info = new AtlasAnalysisInfo(_atlasItemList[i]);
            if (!_imageItemDict.TryGetValue(_atlasItemList[i].texture, out var spriteList))
            {
                atlasAnalysisInfoDict.Add(_atlasItemList[i].texture, info);
                continue;
            }

            foreach (var imagePackageItem in spriteList)
            {
                info.spriteTotalSize += imagePackageItem.texture.width * imagePackageItem.texture.height;
            }

            atlasAnalysisInfoDict.Add(_atlasItemList[i].texture, info);
        }

        for (var i = 0; i < _movieClipItemList.Count; i++)
        {
            AtlasAnalysisInfo targetInfo = null;
            if (_movieClipItemList[i].texture != null)
            {
                var texture = _movieClipItemList[i].texture.root;
                if (!atlasAnalysisInfoDict.TryGetValue(texture, out targetInfo)) continue;
            }

            if (null == targetInfo) continue;

            foreach (var frame in _movieClipItemList[i].frames)
            {
                var frameSize = Mathf.CeilToInt(frame.texture.width * frame.texture.height);
                targetInfo.spriteTotalSize += frameSize;
            }
        }

        FUIEditorTool.packageAnalysisInfoList.Add(this);
    }
}


public class AtlasAnalysisInfo
{
    public readonly string id;
    public readonly int width;
    public readonly int height;

    public int spriteTotalSize { get; set; }

    public AtlasAnalysisInfo(PackageItem item)
    {
        id = item.id;
        width = item.texture.width;
        height = item.texture.height;
    }

    public float fillingRate => (float)spriteTotalSize / (height * width);
}

public class UICheckComponentInfo
{
    private readonly PackageItemType _packageItemType;
    private readonly string _packageItemName;

    private readonly FUIEditorTool.FUIReasonEnum _fuiReasonEnum;
    private readonly string _useOtherPackageName;

    public string needCheckChildName { get; set; }

    public UICheckComponentInfo(PackageItem item, UIPackage otherPackage,
        FUIEditorTool.FUIReasonEnum type = FUIEditorTool.FUIReasonEnum.UseOtherPackageAssets)
    {
        _packageItemType = item.type;
        _packageItemName = item.name;
        if (otherPackage != null)
        {
            _useOtherPackageName = otherPackage.name;
        }

        _fuiReasonEnum = type;
    }

    public UICheckComponentInfo(PackageItem item, FUIEditorTool.FUIReasonEnum reasonType)
    {
        _packageItemType = item.type;
        _packageItemName = item.name;
        _fuiReasonEnum = reasonType;
    }

    public override string ToString()
    {
        switch (_packageItemType)
        {
            case PackageItemType.Image:
                return
                    $"    │   ├─── {"[Image]".PadRight(13, ' ')} {_packageItemName.PadRight(20, ' ')} {FUIEditorTool.FUI_REASONS[(int)_fuiReasonEnum]}";
            case PackageItemType.MovieClip:
                return
                    $"    │   ├─── {"[MovieClip]".PadRight(13, ' ')} {_packageItemName.PadRight(20, ' ')} {FUIEditorTool.FUI_REASONS[(int)_fuiReasonEnum]}";
            case PackageItemType.Component:
                if (string.IsNullOrEmpty(needCheckChildName))
                {
                    var a = "[Component]".PadRight(13, ' ');
                    var b = _packageItemName.PadRight(20, ' ');
                    var d = FUIEditorTool.FUI_REASONS[(int)_fuiReasonEnum];
                    return $"    │   ├─── {a} {b} {d}";
                }
                else
                {
                    var a = "[Component]".PadRight(13, ' ');
                    var b = _packageItemName.PadRight(20, ' ');
                    var c = needCheckChildName.PadRight(12, ' ');
                    var d = FUIEditorTool.FUI_REASONS[(int)_fuiReasonEnum];
                    return $"    │   ├─── {a} {b} -> {c} {d}";
                }
        }

        return "    │   ├───Error Type Info";
    }
}