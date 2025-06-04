namespace Blog.Api;

public class ServiceResponse {
	public object Data { get; set; }
	public string Message { get; set; } = "Başarılı";
	public bool Status { get; set; } = true;
}
