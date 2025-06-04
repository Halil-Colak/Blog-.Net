$(function () {
	$('.selectBox').select2({
		placeholder: "Seçim yapınız",
		allowClear: true,
		closeOnSelect: false,
		language: {
			noResults: function () {
				return "Sonuç bulunamadı.";
			},
			searching: function () {
				return "Aranıyor...";
			}
		}
	});


    function AlertJS(isSuccess, message) {
        $("#AlertJS").removeClass("success failed");
        var iconClass = isSuccess ? "fa-circle-check" : "fa-circle-xmark";
        var statusText = isSuccess ? "Başarılı" : "Başarısız";
        var statusClass = isSuccess ? "success" : "failed";

        $("#AlertJS").addClass(statusClass);
        $("#AlertJS").html(
            '<span>' + message + '</span>'
        );

        $("#AlertJS").css("top", "13px");
        $("#AlertJS").css("opacity", "1");

        setTimeout(function () {
            $("#AlertJS").css("top", "-100px");
            $("#AlertJS").css("opacity", "0");
        }, 3000);
    }

})