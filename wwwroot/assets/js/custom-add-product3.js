(function () {

    //  If Quill is not loaded, stop immediately
    if (typeof Quill === "undefined") {
        console.warn("Quill not loaded – editors skipped");
        return;
    }

    // snow editor
    const editor5El = document.querySelector("#editor5");
    if (editor5El) {
        window.editor5 = new Quill(editor5El, {
            modules: { toolbar: "#toolbar5" },
            theme: "snow",
            placeholder: "Enter product details...",
        });
    }

    // bubble editor
    const editor6El = document.querySelector("#editor6");
    if (editor6El) {
        window.editor6 = new Quill(editor6El, {
            modules: { toolbar: "#toolbar6" },
            theme: "bubble",
            placeholder: "Enter product details...",
        });
    }

    // snow editor 7 (global)
    const editor7El = document.querySelector("#editor7");
    if (editor7El) {
        window.editor7 = new Quill(editor7El, {
            modules: { toolbar: "#toolbar7" },
            theme: "snow",
            placeholder: "Enter product details...",
        });
    }

    // arabic editor 8
    const editor8El = document.querySelector("#editor8");
    if (editor8El) {
        window.editor8 = new Quill(editor8El, {
            modules: { toolbar: "#toolbar8" },
            theme: "snow",
            placeholder: "أدخل تفاصيل المنتج...",
        });
    }

})();

//(function () {
//    // snow editor
//    var editor5 = new Quill("#editor5", {
//        modules: { toolbar: "#toolbar5" },
//        theme: "snow",
//        placeholder: "Enter your messages...",
//    });

//    // bubble editor
//    var editor6 = new Quill("#editor6", {
//        modules: { toolbar: "#toolbar6" },
//        theme: "bubble",
//        placeholder: "Enter your messages...",
//    });

//    // Make the editors global
//    window.editor7 = new Quill("#editor7", {
//        modules: { toolbar: "#toolbar7" },
//        theme: "snow",
//        placeholder: "Enter your messages...",
//    });

//    window.editor8 = new Quill("#editor8", {
//        modules: { toolbar: "#toolbar8" },
//        theme: "snow",
//        placeholder: "أدخل رسالتك",
//    });

//})();
