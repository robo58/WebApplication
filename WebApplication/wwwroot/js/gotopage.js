﻿$(function () {
    $(".pagebox").click(function () {
        $(this).select(); 
    });

    function validRange(str, min, max) {
        let intRegex = /^\d+$/;
        if(intRegex.test(str))
        {
            let num = parseInt(str);
            return num >= min && num <= max;
        }
        else
            {
            return false;
        }
    }

    $(".pagebox").bind('keyup',function (event) {
        let keycode = event.keyCode ? event.keyCode : event.which;
        let pagebox = $(this);
        if(keycode === 13)
        {
            if(validRange(pagebox.val(), pagebox.data('min'), pagebox.data('max')))
            {
                let link = pagebox.data('url');
                link = link.replace('-1', pagebox.val());
                window.location = link;
            }
        }
        else if(keycode === 27)
        {
            pagebox.val(pagebox.data('current'));
        }
    });
});