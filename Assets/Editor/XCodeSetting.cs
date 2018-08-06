using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;

public class XcodeSettings {

  [PostProcessBuildAttribute(0)]
  public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject) {

    // Stop processing if target is NOT iOS
    if (buildTarget != BuildTarget.iOS)
      return;

    // Initialize PbxProject
    var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
    PBXProject proj = new PBXProject();
    proj.ReadFromFile(projectPath);
    string target = proj.TargetGuidByName("Unity-iPhone");

    // need libresolv for gRPC
    proj.AddFileToBuild(target, proj.AddFile("usr/lib/libresolv.9.dylib", "Frameworks/libresolv.9.dylib", PBXSourceTree.Sdk));

    // Apply settings
    File.WriteAllText(projectPath, proj.WriteToString());

  }
}