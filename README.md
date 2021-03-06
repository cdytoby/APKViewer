# APKViewer

A simple apk file viewer, developed using .Net Standard and .Net Framework.

![alt text](/ReadmeAssets/ScreenShot_Main.png)

### Features

- Support apk, aab and ipa file (ipa is not yet fully supported)
- Drag and Drop file to open it
- View basic information incl. package name, icon, version, sdk, etc
- View Sign signiture
- View file hash
- Open in Google Play Store
- install apk file using adb

### To all passerby

Feel free to create issues for bug-report, feature-request, improvement-suggestion and so on. Especially if the project is many years old and has issue with newer OS or some special apks.

I need help for making macOS version. If you think you can do this and want to support this project, go for it and feel free to make Pull requests. I have a macOS machine to test in my workplace, and I appretiate your support.

### My Plan

All bug fix will be done as soon as possible if I have time. Here are the features I'm considering to develop.

- Multi-platform Support for MacOS and Linux. (I have zero experience on MacOS and Linux development)
- UI Multi-language

But I don't think I'll do the following features.
- Rename file
- open in other apk website like apkpure
- Hash Compare
- Decompile or unpack apk
- sign or resign apk or aab file
- extract apk or aab file content

### About this project

There is already a very good APK-Info out there, and it contains more features.

The reason I do this because:
- I don't have any GitHub open source project experience, and this is my very first one.
- I don't trust AutoIt, I noticed that many exes export from it will be recognised as virus. That's probably faulty report, but I still don't trust this tool.
- I'm using .Net Standard for potencial multiplatform approach. I want it go multiplatform. And c# with .Net Standard is the only knowledge I have to create a multiplatform app with good UI.

### License

This software is licensed under MIT License. 

https://github.com/cdytoby/APKViewer/blob/master/LICENSE

Bundletool is licensed under Apache License 2.0

https://github.com/google/bundletool/blob/master/LICENSE

ADB and AAPT is licensed under Apache License 2.0

https://github.com/cdytoby/APKViewer/blob/master/ExternalTools/Windows/NOTICE.txt

The app icon is from ionic-team, and it is modified by me, the color is changed to green.

https://github.com/ionic-team/ionicons

https://ionicons.com/
