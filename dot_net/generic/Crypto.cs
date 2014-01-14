using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;

/// <summary>
/// Summary description for Crypto
/// </summary>
namespace business
{
  public class Crypto
  {
    private static byte[] _salt = Encoding.ASCII.GetBytes("v5evBX190x1fe8NI1PPEX7HMKwJS2LA4");

    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using 
    /// DecryptStringAES().  The sharedSecret parameters must match.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
    public static string EncryptStringAES(string plainText, string sharedSecret)
    {
      if (string.IsNullOrEmpty(plainText))
        throw new ArgumentNullException("plainText");
      if (string.IsNullOrEmpty(sharedSecret))
        throw new ArgumentNullException("sharedSecret");

      string outStr = null;                       // Encrypted string to return
      RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

      try
      {
        // generate the key from the shared secret and the salt
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

        // Create a RijndaelManaged object
        // with the specified key and IV.
        aesAlg = new RijndaelManaged();
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
        aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

        // Create a decrytor to perform the stream transform.
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for encryption.
        using (MemoryStream msEncrypt = new MemoryStream())
        {
          using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {

              //Write all data to the stream.
              swEncrypt.Write(plainText);
            }
          }
          outStr = Convert.ToBase64String(msEncrypt.ToArray());
        }
      }
      finally
      {
        // Clear the RijndaelManaged object.
        if (aesAlg != null)
          aesAlg.Clear();
      }

      // Return the encrypted bytes from the memory stream.
      return outStr;
    }

    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using 
    /// EncryptStringAES(), using an identical sharedSecret.
    /// </summary>
    /// <param name="cipherText">The text to decrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
    public static string DecryptStringAES(string cipherText, string sharedSecret)
    {
      if (string.IsNullOrEmpty(cipherText))
        throw new ArgumentNullException("cipherText");
      if (string.IsNullOrEmpty(sharedSecret))
        throw new ArgumentNullException("sharedSecret");

      // Declare the RijndaelManaged object
      // used to decrypt the data.
      RijndaelManaged aesAlg = null;

      // Declare the string used to hold
      // the decrypted text.
      string plaintext = null;

      try
      {
        // generate the key from the shared secret and the salt
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

        // Create a RijndaelManaged object
        // with the specified key and IV.
        aesAlg = new RijndaelManaged();
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
        aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

        // Create a decrytor to perform the stream transform.
        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        // Create the streams used for decryption.                
        byte[] bytes = Convert.FromBase64String(cipherText);
        using (MemoryStream msDecrypt = new MemoryStream(bytes))
        {
          using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))

              // Read the decrypted bytes from the decrypting stream
              // and place them in a string.
              plaintext = srDecrypt.ReadToEnd();
          }
        }
      }
      finally
      {
        // Clear the RijndaelManaged object.
        if (aesAlg != null)
          aesAlg.Clear();
      }

      return plaintext;
    }


    /** Taken from http://stackoverflow.com/questions/1278974/php-encryption-vb-net-decryption start **/
    public const String CRYTO_SHARED_KEY = "WO0J7a8a4h5D6448F66MOeS6Bo9JzE24"; // 32 * 8 = 256 bit key
    public const String CRYTO_SHARED_IV = "4DRKCmF5TneKfP3A7klO15Kh7r1xfN5y"; // 32 * 8 = 256 bit key

    public static string DecryptRJ256(string prm_key, string prm_iv, string prm_text_to_decrypt)
    {
      string sEncryptedString = prm_text_to_decrypt;

      RijndaelManaged myRijndael = new RijndaelManaged();
      myRijndael.Padding = PaddingMode.Zeros;
      myRijndael.Mode = CipherMode.CBC;
      myRijndael.KeySize = 256;
      myRijndael.BlockSize = 256;

      byte[] key;
      byte[] IV;

      key = System.Text.Encoding.ASCII.GetBytes(prm_key);
      IV = System.Text.Encoding.ASCII.GetBytes(prm_iv);

      ICryptoTransform decryptor = myRijndael.CreateDecryptor(key, IV);

      byte[] sEncrypted = Convert.FromBase64String(sEncryptedString);
      byte[] fromEncrypt = new byte[sEncrypted.Length];

      MemoryStream msDecrypt = new MemoryStream(sEncrypted);
      CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
      csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

      return (System.Text.Encoding.ASCII.GetString(fromEncrypt));
    }


    public static string EncryptRJ256(string prm_key, string prm_iv, string prm_text_to_encrypt)
    {
      string sToEncrypt = prm_text_to_encrypt;

      RijndaelManaged myRijndael = new RijndaelManaged();
      myRijndael.Padding = PaddingMode.Zeros;
      myRijndael.Mode = CipherMode.CBC;
      myRijndael.KeySize = 256;
      myRijndael.BlockSize = 256;

      byte[] encrypted;
      byte[] toEncrypt;
      byte[] key;
      byte[] IV;

      key = System.Text.Encoding.ASCII.GetBytes(prm_key);
      IV = System.Text.Encoding.ASCII.GetBytes(prm_iv);

      ICryptoTransform encryptor = myRijndael.CreateEncryptor(key, IV);

      MemoryStream msEncrypt = new MemoryStream();
      CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

      toEncrypt = System.Text.Encoding.ASCII.GetBytes(sToEncrypt);

      csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
      csEncrypt.FlushFinalBlock();

      encrypted = msEncrypt.ToArray();

      return (Convert.ToBase64String(encrypted));
    }
    /** Taken from http://stackoverflow.com/questions/1278974/php-encryption-vb-net-decryption end **/

  }



}