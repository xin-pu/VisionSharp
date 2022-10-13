# vision_sharp
基于Opencv模块封装的的机器视觉库


## TextDetector

### 二维码检测器 BarcodeDetector

```C#
var barcodeDetector = new BarcodeDetector();
var mat = Cv2.ImRead(@"..\..\..\..\testimages\barcode.png");
var code = barcodeDetector.Call(mat);
```
