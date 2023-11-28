# MinaSignerNet

A sdk to sign message for mina blockchain with dotnet framework, project in construction

Net standard 2.0 library compatibility : [https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)

Created with Visual Studio 2022

## Compile the dll
Right click on MinaSignerNet and choose publish, click on publish button on the upper right

You need to copy Blake2Core.dll on MinaSignerNet.dll in order to use it

## Sign and verify

Actually you can sign BigInteger or List of BigInteger variable and verify the signature

Exemple of signature 
```
 string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
 BigInteger message = BigInteger.Parse("123456");          
 Signature signature = Signature.Sign(message, privKey, Network.Testnet);
```

How to verify it 
```
 string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
 BigInteger message = BigInteger.Parse("123456");  
 Signature signature = Signature.Sign(message, privKey, Network.Testnet);
 var isGood = Signature.Verify(signature, message, pubKey, Network.Testnet);
```

## Private key and Public key from base 58
```
 string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
 PrivateKey pKey = new PrivateKey(privKey);
 var pubKey = pKey.GetPublicKey();
```

```
 string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";
 PublicKey key = new PublicKey(pubKey);
```

The method toString from Signature/PublicKey/PrivateKey return the base58 encoded version

## Auth Exemple

MinaAspAuth is an asp.net Api project whi implement mina signer to authenticate user based on signature generate by a wallet like Auro Wallet.

You can see how to generate a bearer Token based on signature and account address, usefull to implement product/service for Mina users.

