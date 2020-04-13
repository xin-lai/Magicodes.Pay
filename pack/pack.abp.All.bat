call ./clear.bat

call ./pack.bat "Magicodes.Pay.Abp.*" "../src/Magicodes.Pay.Abp/Magicodes.Pay.Abp.csproj"

call ./pack.bat "Magicodes.Pay.Abp.Alipay.*" "../src/Magicodes.Pay.Abp.Alipay/Magicodes.Pay.Abp.Alipay.csproj"

call ./pack.bat "Magicodes.Pay.Abp.Allinpay.*" "../src/Magicodes.Pay.Abp.Allinpay/Magicodes.Pay.Abp.Allinpay.csproj"

call ./pack.bat "Magicodes.Pay.Abp.Wxpay.*" "../src/Magicodes.Pay.Abp.Wxpay/Magicodes.Pay.Abp.Wxpay.csproj"

call ./pack.bat "Magicodes.Pay.Abp.Alipay.Global.*" "../src/Magicodes.Pay.Abp.Alipay.Global/Magicodes.Pay.Abp.Alipay.Global.csproj"

@pause