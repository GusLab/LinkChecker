var modal_dlg;
var active_color = '#000'; // Colour of user provided text
var inactive_color = '#999'; // Colour of default text

$j(document).ready(function () {
	$j("input.default-value").css("color", inactive_color);
	$j("input.default-value").val($j("input.default-value").attr('placeholder')); 
	
	var default_values = new Array();
	
	$j("input.default-value").blur(function () {
		if (this.value == '') {
			$j("input.default-value").css("color", inactive_color);
			this.value = $j(this).attr('placeholder');
		}
	});
	$j("input.default-value").focus(function () {
		if (!default_values[this.id]) {
			default_values[this.id] = this.value;
		}
		if (this.value == default_values[this.id]) {
			this.value = '';
			this.style.color = active_color;
		}
		$j(this).blur(function () {
			if (this.value == '') {
				this.style.color = inactive_color;
				this.value = default_values[this.id];
			}
		});
	});
		$j('#domain_name').live('click', function () {
				$j('.domain_error_content').fadeOut('normal');
			});

		$j('#domain_name').live('keydown', function (e) {
				if (e.keyCode == 13) {
					$j('#search').click();
				}
			});

		 $j('#search').live('click', function () {
				$j('.domain_error_content').html('').fadeOut();
				CountSearch = 0;
				DomainInputCheck();
			});

		$j('#choose_domain').live('click', function () {
				ChooseDomains();
			});

		$j('input[name=domains_dot]').live('change', function () {
				ShowSearchGroup($j(this).val());
			});
			
modal_dlg = $j('<div></div>')
		.dialog({
		    autoOpen: false,
		    title: 'Loading...',
		    modal: true,
		    position: 'top',
		    closeOnEscape: true,
		    closeText: 'hide',
		    resizable: false,
		    draggable: false,
		    width: 700,
		    height: 600
		}).html("<div align='center' style='width:100%;'>Please wait...</div>");

});

var Modalbox = new Object();

Modalbox.show = function (url_or_html, options) {
    modal_dlg.dialog({ dialogClass: "flora" });
    $j('.flora.ui-dialog').css({ "position": "fixed" }).css({"display": "block"});

    modal_dlg.dialog('close');
    modal_dlg.html("<div align='center' style='width:100%;'>Please wait...</div>")
             .dialog("option", "title", "Loading... ").dialog("option", "position", "top")
             .dialog('open');
    var html = "";
    var pattern = /<\/?[^>]+>/gi;
    if (pattern.test(url_or_html) == false) {
        $j.ajax({ url: url_or_html, asynch: false, success: function (response) {
            html = response;
            modal_dlg.dialog('close');
            if (options == undefined) {
                options = new Array();
            }
            try {
                if (options['height'] == undefined) {
                    options['height'] = modal_dlg.dialog("option", "height");
                }
            } catch (e) {
                options['height'] = modal_dlg.dialog("option", "height");
            }

            try {
                if (options['width'] == undefined) {
                    options['width'] = modal_dlg.dialog("option", "width");
                }
            } catch (e) {
                options['width'] = modal_dlg.dialog("option", "width");
            }

            try {
                if (options['title'] == undefined) {
                    options['title'] = modal_dlg.dialog("option", "title");
                }
            } catch (e) {
                options['title'] = modal_dlg.dialog("option", "title");
            }

            modal_dlg.dialog("option", "height", options.height)
                     .dialog("option", "width", options.width)
                     .dialog("option", "title", options.title)

            modal_dlg.html(html);
            modal_dlg.dialog('open');
        }
        });
    } else {
        html = url_or_html;
        modal_dlg.dialog('close');
        if (!options.height) {
            options['height'] = modal_dlg.dialog("option", "height");
        }
        if (!options.width) {
            options['height'] = modal_dlg.dialog("option", "width");
        }
        if (!options.title) {
            options['height'] = modal_dlg.dialog("option", "title");
        }
        modal_dlg.dialog("option", "height", options.height)
                 .dialog("option", "width", options.width)
                 .dialog("option", "title", options.title)

        modal_dlg.html(html);
        modal_dlg.dialog('open');
    }

}

Modalbox.hide = function() {
    modal_dlg.dialog('close');    
}

function ShowSearchGroup(id) {
    try {
        var hodden = $j('.search_group').addClass('hidden');
        $j('#s_group_' + id).removeClass('hidden');
    }
    catch (e) {
        alert(e);
    }
}

function DomainInputCheck() {

    var pat2 = /[\n\r ,]/g;
    var validator = /[~`!@#$%^&*()_+=\:;\/\"\[\]\+\=?\'\|]/g;
    var search_query = $j("#domain_name").val();
    if (search_query == "" || search_query == $j("#domain_name").attr('default_value')) {
        $j('#domain_name').addClass('wsform_border_invalid');
        $j('.domain_error_content').html("Please type in at least one domain name.").fadeIn();
        return false;
    }
    var check_text = "";

    var tmp_domain_name = new Array();
    var domain_name = new Array();

    var pat2 = /[\n\r ,]/g;
    var d_names_with_dot = "";
    var d_names_without_dot = "";
    domain_name = search_query.split(pat2);
    var domain_result = eliminateDuplicates(domain_name);
    var dwd_count = 0;
    for (var i = 0; i < domain_result.length; i++) {
        if (domain_result[i] != "") {

            if (validator.test(domain_result[i])) {
                check_text += "* " + domain_result[i].replace(/</g, '&lt').replace(/>/g, '&gt') + ": contain not allowed characters<br/>";
            }
            if (domain_result[i][0] == "-" || (domain_result[i][2] == "-" && domain_result[i][3] == "-")) {
                check_text += '* ' + domain_result[i].replace(/</g, '&lt').replace(/>/g, '&gt') + ': "-" (hyphen) cannot be the first character, or both 3rd and 4th characters<br/>';
            }
            if (domain_result[i].length > 240) {
                check_text += '* domain name ' + domain_result[i].substring(0, 10) + "... is to long";
            }
            if (domain_result[i].indexOf(".") == -1) {
                d_names_without_dot += domain_result[i].replace(/</g, '&lt').replace(/>/g, '&gt') + ",";
                dwd_count++;
            }
            else {
                d_names_with_dot += domain_result[i].replace(/</g, '&lt').replace(/>/g, '&gt') + ",";


            }
        }
    }
    if (check_text != "") {
        $j('#domain_name').addClass('wsform_border_invalid');
        if (check_text.length > 500) {
            check_text = check_text.substring(0, 500) + "...";
        }
        $j('.domain_error_content').html("Input domain names are incorrect, details are listed bellow:<br />" + check_text).fadeIn();
        return false;
    }

    $j('#d_names_with_dot').val(d_names_with_dot);
    $j('#d_names_without_dot').val(d_names_without_dot);
    if (d_names_without_dot != "") {
        $j("#check").val(1);
    }
    if (dwd_count > 1) {
    /*    var params = "&d_names_with_dot=" + d_names_with_dot + "&d_names_without_dot=" + d_names_without_dot;

        Modalbox.show(
                    "https://clients.web-solutions.eu/cart/domains/searchoptions.html?" + params,
                    {
                        title: "Choose domains to search",
                        width: 500,
                        height: 'auto'
                    }
                ); */
				$j('#d_names_without_dot').val(d_names_without_dot.substring(0,d_names_without_dot.indexOf(',')));
				StartSearch();
    }
    else {
        StartSearch();
    }
}

function ChooseDomains() {
    var domain_name = $j('#domains_dot:checked').val();
    $j('#d_names_without_dot').val(domain_name);
    $j('#s_group').val($j("#search_group_" + domain_name).val());
    $j('#check').val(1);
    StartSearch();
    Modalbox.hide();
}

function eliminateDuplicates(arr) {
    var i,
      len = arr.length,
      out = [],
      obj = {};

    for (i = 0; i < len; i++) {
        obj[arr[i]] = 0;
    }
    for (i in obj) {
        out.push(i);
    }
    return out;
}

function StartSearch() {
	$j('#search_hash').val(MakeSearchHash());
    var params = "action=start_search&" + $j('#search_form').serialize();
	 $j.ajax({
        type: 'POST',
        data: params,
        url: 'https://clients.web-solutions.eu/cart/domains/ajax.html',
        async: false
		});
	$j('#search_form').submit();
}
function MakeSearchHash(){
var date = new Date();

var search_hash = date.getFullYear() + "-" +
                  (date.getMonth().toString().length == 1 ? "0" + (date.getMonth() + 1).toString() : (date.getMonth() + 1).toString()) + "-" + date.getDate() + " " + (date.getHours().toString().length == 1 ? "0" + date.getHours() : date.getHours()) +  ":" + (date.getMinutes().toString().length == 1 ? "0" + date.getMinutes() : date.getMinutes()) + ":" + (date.getSeconds().toString().length == 1 ? "0" + date.getSeconds() : date.getSeconds()) + "." + date.getMilliseconds();

var random = Math.random().toString();
random = random.substring(random.indexOf('.') +1,10)

return search_hash = search_hash + "." + random;
}