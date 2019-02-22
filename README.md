# UnityReferencesTree
用于以树形结构显示Unity3D中，指定物体的所有引用层级关系(编辑器工具)

# 使用

对着需要查找引用的物体，点击鼠标右键->FindRefernces即可。

除此之外，在菜单上的Tools->Open Refernces Find Window也可以直接打开该界面...就是这样。

我放在Github上的场景，添加了一个示例，右键Example中的Object文件，便可以查看到如下引用：

![引用示例](https://github.com/CWHISME/UnityReferencesTree/raw/master/Image/Snipaste_2019-02-01_17-41-46.png)

另外，菜单上还有几个选项，比如查找模式和显示模式等。
显示模式主要影响显示排版，而查找模式....则用于显示引用文件的目录结构。

每个被引用的物体会显示两个按钮“定位”、“查找位置”。
“定位” 可以直到引用物体的项目位置
“查找位置” 可以直接找出主物体上，哪些GameObject使用了该引用物体。

更多详情可查看项目Example。