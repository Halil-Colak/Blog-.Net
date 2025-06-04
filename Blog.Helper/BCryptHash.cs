namespace Blog.Helper;
public static class BCryptHash {
	//Kullanıcıdan alınan düz metin (plain text) şifreyi güvenli bir şekilde hash'ler.
	public static string BCryptHashPassword(string passwordHash) => BCrypt.Net.BCrypt.HashPassword(passwordHash);
	//Kullanıcıdan gelen düz metin şifreyi, veritabanında saklanan hash'lenmiş şifreyle karşılaştırır.
	public static bool VerifyPassword(string plainPassword, string hashedPassword) => BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
}

