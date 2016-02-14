# qrcodr
A qr code api using a custom asp.net core middleware built on asp.net core and using QRCode.Net (https://qrcodenet.codeplex.com/).

usage: http://[yourwebsite]/api/qrcode?text=[text to encode]&size[1-100]&format[jpeg,png,gif]

once a qrcode is created, it is cached
