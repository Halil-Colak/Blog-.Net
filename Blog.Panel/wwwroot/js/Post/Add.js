//#region Yazı Sitilleri
// Seçili alana stil uygula
function applyCommand(command, value = null) {
	document.execCommand(command, false, value);
}

// Stil ekleme veya kaldırma
function toggleStyle(styleProperty, styleValue) {
	const selection = window.getSelection();
	const range = selection.getRangeAt(0);
	const selectedText = range.toString();

	if (selectedText) {
		const span = document.createElement("span");
		span.style[styleProperty] = styleValue;

		// Seçili metin daha önce bir stil içeriyorsa, mevcut stili koruyarak ekle
		const parentNode = range.commonAncestorContainer.parentNode;
		if (parentNode.nodeName === "SPAN") {
			span.style.cssText = parentNode.style.cssText; // Mevcut stilleri koru
			span.style[styleProperty] = styleValue; // Yeni stili uygula
		}

		span.textContent = selectedText;
		range.deleteContents();
		range.insertNode(span);
	}
}

// Araç çubuğu butonları
$("#BoldBtn").on("click", function () {
	toggleStyle("fontWeight", "bold");
});
$("#ItalicBtn").on("click", function () {
	toggleStyle("fontStyle", "italic");
});
$("#UnderlineBtn").on("click", function () {
	applyCommand("underline");
});
$("#ClearFormattingBtn").on("click", function () {
	document.execCommand("removeFormat", false, null);
});
// Üst ve alt çizgi ekleme
$("#StrikethroughBtn").on("click", function () {
	const selection = window.getSelection();
	const selectedText = selection.toString();

	if (selectedText) {
		const span = document.createElement("span");
		span.style.textDecoration = "line-through";
		span.textContent = selectedText;
		const range = selection.getRangeAt(0);
		range.deleteContents();
		range.insertNode(span);
	}
});
//#endregion

//#region Popup

// Kod bloğu ekleme popup açma
$("#AddCodeBlockBtn").on("click", function () {
	$("#CodePopup").fadeIn().css("display", "flex");
});

// Liste ekleme popup açma
$("#AddListBtn").on("click", function () {
	$("#ListPopup").fadeIn().css("display", "flex");
});

// Resim ekleme popup açma
$("#AddImageBtn").on("click", function () {
	$("#ImagePopup").fadeIn().css("display", "flex");
});
// Kod popup kapama
$("#ClosePopupBtn").on("click", function () {
	$("#CodePopup").fadeOut(200);
});

// Liste popup kapama
$("#CloseListPopupBtn").on("click", function () {
	$("#ListPopup").fadeOut(200);
});

// Resim popup kapama
$("#CloseImagePopupBtn").on("click", function () {
	$("#ImagePopup").fadeOut(200);
});


// Link popup kapama
$("#CloseLinkPopupBtn").on("click", function () {
	$("#LinkPopup").fadeOut(200);
});

// Link ekleme popup açma
$("#AddLinkBtn").on("click", function () {
	$("#LinkPopup").fadeIn().css("display", "flex");
});
//#endregion

//#region İşlemler


// Kod ekleme
$("#InsertCodeBtn").on("click", function () {
	const language = $("#LanguageSelect").val();
	const code = $("#CodeInput").val();
	if (code) {
		const preTag = `<br><br><br><pre><code class="language-${language}">${$("<div>").text(code).html()}</code></pre><br><br><br><br><br><br><br><br><br>`;
		$("#EditorBox").append(preTag);
		$("pre code").each(function () {
			hljs.highlightElement(this);
		});
		$("#CodePopup").fadeOut(200);
	} else {
		alert("Lütfen kodu girin.");
	}
});

// Resim ekleme
$("#InsertImageBtn").on("click", function () {
	const file = $("#ImageInput")[0].files[0];
	if (file) {
		const reader = new FileReader();
		reader.onload = function (e) {
			const imgTag = `<br><br><br><img src="${e.target.result}" alt="Resim" class="picture-popup" style="width: 100%;max-width: 100%;max-height: 450px;object-fit: contain;"><br><br><br>`;
			$("#EditorBox").append(imgTag);
		};
		reader.readAsDataURL(file);
	}
	$("#ImagePopup").fadeOut(200);
});

// Liste ekleme işlemi
$("#InsertListBtn").on("click", function () {
	const listItems = $("#ListInput").val().split("\n").filter(item => item.trim() !== "");
	if (listItems.length > 0) {
		const listTag = `<ul>${listItems.map(item => `<li>${item}</li>`).join("")}</ul><br><br><br>`;
		$("#EditorBox").append(listTag);
		$("#ListPopup").fadeOut(200);
	} else {
		alert("Lütfen liste öğelerini girin.");
	}
});

// Bordürlü kutu ekleme
$("#BorderBoxBtn").on("click", function () {
	const div = $("<br><br><div></div><br><br>").addClass("border-box").text("Border Yazılı Alanı");
	$("#EditorBox").append(div);
});

// HTML Çıktısını al
$("#HtmlOutputBtn").on("click", function () {
	const htmlContent = $("#EditorBox").html();
	$("#OutputArea").text(htmlContent);
});

// Link ekleme
$("#InsertLinkBtn").on("click", function () {
	const url = $("#urlInput").val();
	const linkText = $("#linkTextInput").val();

	if (url && linkText) {
		const linkTag = `<a href="${url}" target="_blank" style="color:#5050fd;text-decoration:underline;">${linkText}</a>`;

		// #EditorBox içerisine linkTag'ı ekle
		$("#EditorBox").append(linkTag);

		// Popup'ı gizle
		$("#LinkPopup").fadeOut(200);
	} else {
		alert("Lütfen URL ve Link Metnini girin.");
	}
});

// Başlık Seçimi
$('#HeadingSelect').on('change', function () {
	const headingTag = $(this).val(); // Kullanıcının seçtiği başlık etiketi
	const selection = window.getSelection();
	if (!selection.rangeCount) return;

	const range = selection.getRangeAt(0);

	if (range.toString()) {
		// Seçili metnin kapsayıcı öğesini bul
		let parentElement = range.commonAncestorContainer.parentElement;

		// Eğer mevcut bir başlık etiketi varsa, sadece metni al ve başlık etiketini kaldır
		if (/^H[1-6]$/.test(parentElement.tagName)) {
			const cleanText = parentElement.textContent; // Mevcut başlıktaki metin
			const newNode = document.createElement(headingTag);
			newNode.textContent = cleanText;

			// Eski başlık etiketini yenisiyle değiştir
			parentElement.replaceWith(newNode);
		} else {
			// Eğer başlık etiketi yoksa doğrudan yeni bir başlık ekle
			const selectedText = range.toString();
			const newNode = document.createElement(headingTag);
			newNode.textContent = selectedText;

			range.deleteContents(); // Seçili alanı temizle
			range.insertNode(newNode); // Yeni başlık etiketini ekle
		}
		selection.removeAllRanges(); // Seçimi sıfırla
	}
});

// Arka Plan Rengi Seçimi
$('#BgColorSelect').on('change', function () {
	const color = $(this).val(); // Seçilen arka plan rengi
	const selection = window.getSelection();
	const range = selection.getRangeAt(0);

	if (range.toString()) {
		const spanNode = document.createElement('span');
		spanNode.style.backgroundColor = color;
		spanNode.style.borderRadius = '5px'; // Kenar yumuşatma
		spanNode.style.padding = '2px 4px'; // İç boşluk
		spanNode.textContent = range.toString();
		range.deleteContents();
		range.insertNode(spanNode);
	}
});


$("#SpaceBtn").on("click", function () {
	// EditorBox'a boş satır ekleyin
	const emptyLine = "<br><br>";
	$("#EditorBox").append(emptyLine);
});
//#endregion



//#region submit
$(document).ready(function () {
	$("#myForm").on("submit", function () {
		// İçeriği gizli input'a aktar
		$("#Content").val($("#EditorBox").html());
	});
});


//#endregion