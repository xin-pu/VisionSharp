# vision_sharp
����Opencvģ���װ�ĵĻ����Ӿ���


## TextDetector

### ��ά������ BarcodeDetector

```C#
var barcodeDetector = new BarcodeDetector();
var mat = Cv2.ImRead(@"..\..\..\..\testimages\barcode.png");
var code = barcodeDetector.Call(mat);
```
