using System.Text;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Domain.Cryptography;

public class DigitalSign
{
    public static SignResult SignText(string text)
    {
        var gen = new ECKeyPairGenerator();
        
        var ecSpec=SecNamedCurves.GetByName("secp256r1");
        var domainParams=new ECDomainParameters(ecSpec.Curve,ecSpec.G,ecSpec.N,ecSpec.H);
        
        gen.Init(new ECKeyGenerationParameters(domainParams,new SecureRandom()));
        
        var keyPair = gen.GenerateKeyPair();

        //cert gen
        var certGen = new X509V3CertificateGenerator();
        certGen.SetSerialNumber(BigInteger.ProbablePrime(120, new Random()));
        
        var issuer=new X509Name("CN=Test");
        certGen.SetIssuerDN(issuer);
        certGen.SetSubjectDN(issuer);

        certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
        certGen.SetNotAfter(DateTime.UtcNow.AddYears(1));

        certGen.SetPublicKey(keyPair.Public);
        
        var signFactory=new Asn1SignatureFactory("SHA256withECDSA",keyPair.Private);
        var cert=certGen.Generate(signFactory);

        var certPem = ToPem("CERTIFICATE", cert.GetEncoded());
        
        //sign text
        var data=Encoding.UTF8.GetBytes(text);
        var signer=SignerUtilities.GetSigner("SHA256withECDSA");
        signer.Init(true,keyPair.Private);
        signer.BlockUpdate(data,0,data.Length);
        var signature=signer.GenerateSignature();
        
        return new SignResult
        {
            CertificatePem = certPem,
            SignatureBase64 = Convert.ToBase64String(signature)
        };
    }
    
    public static bool VerifySignature(string text, string signatureBase64, string certificatePem)
    {
        var cert = new X509CertificateParser().ReadCertificate(Convert.FromBase64String(ExtractPemBody(certificatePem)));
        var signer = SignerUtilities.GetSigner("SHA256withECDSA");
        signer.Init(false, cert.GetPublicKey());
        signer.BlockUpdate(Encoding.UTF8.GetBytes(text), 0, text.Length);
        return signer.VerifySignature(Convert.FromBase64String(signatureBase64));
    }
    
    private static string ToPem(string label, byte[] bytes)
    {
        string b64 = Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks);
        return $"-----BEGIN {label}-----\n{b64}\n-----END {label}-----";
    }
    
    private static string ExtractPemBody(string pem)
    {
        var cleaned = pem
            .Replace("-----BEGIN CERTIFICATE-----", "")
            .Replace("-----END CERTIFICATE-----", "")
            .Replace("\n", "")
            .Replace("\r", "");

        return cleaned;
    }
}



public class SignResult
{
    public string SignedText { get; set; }
    public string CertificatePem { get; set; }
    public string SignatureBase64 { get; set; }
}