/*
 * jQuery gentleSelect plugin (version 0.1.4.1)
 * http://shawnchin.github.com/jquery-cron
 *
 * Copyright (c) 2010-2013 Shawn Chin.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 *
 * Modified By Felix Ruponen to use Quartz Cron Format
 *
 * GitHub: https://github.com/felixruponen/jquery-cron 
 *
 * More at: http://www.quartz-scheduler.org
 *
 *
 * Requires:
 * - jQuery
 *
 * Usage:
 *  (JS)
 *
 *  // initialise like this
 *  var c = $('#cron').cron({
 *    initial: '9 10 * * *', # Initial value. default = "* * * * *"
 *    url_set: '/set/', # POST expecting {"cron": "12 10 * * 6"}
 *  });
 *
 *  // you can update values later
 *  c.cron("value", "1 2 3 4 *");
 *
 * // you can also get the current value using the "value" option
 * alert(c.cron("value"));
 *
 *  (HTML)
 *  <div id='cron'></div>
 *
 * Notes:
 * At this stage, we only support a subset of possible cron options.
 * For example, each cron entry can only of the following form
 * ( - indicates where numbers should be replaced):
 * - Every minuto : 0 0/1 * * * ?
 * - Every hora   : 0 - 0/1 * * ?
 * - Every dia    : 0 - - * * ?
 * - Every semana   : 0 - - ? * -
 * - Every mes  : 0 - - - * ?
 * - Every ano   : 0 - - - - ? *
 *
 * Ex.
 *    0 5 0/1 * * ?    => Every hora, five minutos past the hora
 *
 */
(function($) {

    var defaults = {
        initial : "0 0/1 * * * ?",
        minutoOpts : {
            minWidth  : 100, // only applies if columns and itemWidth not set
            itemWidth : 30,
            columns   : 4,
            rows      : undefined,
            title     : "Minutos pasada la hora"
        },
        timeHourOpts : {
            minWidth  : 100, // only applies if columns and itemWidth not set
            itemWidth : 20,
            columns   : 2,
            rows      : undefined,
            title     : "Time: Hora"
        },
        domOpts : {
            minWidth  : 100, // only applies if columns and itemWidth not set
            itemWidth : 30,
            columns   : undefined,
            rows      : 10,
            title     : "Día del Mes"
        },
        mesOpts : {
            minWidth  : 100, // only applies if columns and itemWidth not set
            itemWidth : 100,
            columns   : 2,
            rows      : undefined,
            title     : undefined
        },
        dowOpts : {
            minWidth  : 100, // only applies if columns and itemWidth not set
            itemWidth : undefined,
            columns   : undefined,
            rows      : undefined,
            title     : undefined
        },
        timeMinuteOpts : {
            minWidth  : 100, // only applies if columns and itemWidth not set
            itemWidth : 20,
            columns   : 4,
            rows      : undefined,
            title     : "Time: Minuto"
        },
        effectOpts : {
            openSpeed      : 400,
            closeSpeed     : 400,
            openEffect     : "slide",
            closeEffect    : "slide",
            hideOnMouseOut : true
        },
        url_set : undefined,
        customValues : undefined,
        onChange: undefined, // callback function each time value changes
        useGentleSelect: false
    };

    // -------  build some static data -------

    // options for minutos in an hora
    var str_opt_mih = "";
    for (var i = 0; i < 60; i++) {
        var j = (i < 10)? "0":"";
        str_opt_mih += "<option value='" + i + "'>" + j + i + "</option>\n";
    }

    // options for horas in a dia
    var str_opt_hid = "";
    for (var i = 0; i < 24; i++) {
        var j = (i < 10)? "0":"";
        str_opt_hid += "<option value='"+i+"'>" + j + i + "</option>\n";
    }

    // options for dias of mes
    var str_opt_dom = "";
    for (var i = 1; i < 32; i++) {
        if (i == 1 || i == 21 || i == 31) { var suffix = "st"; }
        else if (i == 2 || i == 22) { var suffix = "nd"; }
        else if (i == 3 || i == 23) { var suffix = "rd"; }
        else { var suffix = "th"; }
        str_opt_dom += "<option value='"+i+"'>" + i + suffix + "</option>\n";
    }

    // options for meses
    var str_opt_mes = "";
    var meses = ["Enero", "Febrero", "Marzo", "Abril",
                  "Mayo", "Junio", "Julio", "Agosto",
                  "Septiembre", "Octubre", "Noviembre", "Deciembre"];
    for (var i = 0; i < meses.length; i++) {
        str_opt_mes += "<option value='"+(i+1)+"'>" + meses[i] + "</option>\n";
    }

    // options for dia of semana
    var str_opt_dow = "";
    var dias = ["Domingos", "Lunes", "Martes", "Miercoles", "Jueves",
                "Viernes", "Sabados"];
    for (var i = 0; i < dias.length; i++) {
        str_opt_dow += "<option value='" + (i + 1) +"'>" + dias[i] + "</option>\n";
    }

    // options for period
    var str_opt_period = "";
    var periods = ["minuto", "dia", "semana", "mes", "ano"];
    for (var i = 0; i < periods.length; i++) {
        if (i === 5)
            str_opt_period += "<option value='" + periods[i] + "'> a&ntilde;o</option>\n";
        else
            str_opt_period += "<option value='"+periods[i]+"'>" + periods[i] + "</option>\n";
    }

    // display matrix
    var toDisplay = {
        "minuto": [],
        "hora"   : ["mins"],
        "dia"    : ["time"],
        "semana"   : ["dow", "time"],
        "mes"  : ["dom", "time"],
        "ano"   : ["dom", "mes", "time"]
    };

    var combinations = {
        // Quartz Regex Expressions below                  // "-" indicates digit of one or two numbers that should be replaced with the desired time

        "minuto" : /^0\s(0\/1)\s(\*\s){3}\?$/,             // "0 0/1 * * * ?"
        "hora"   : /^0\s\d{1,2}\s(0\/1)\s(\*\s){2}\?$/,    // "0 - 0/1 * * ?"
        "dia"    : /^0\s(\d{1,2}\s){2}(\*\s){2}\?$/,       // "0 - - * * ?"
        "semana"   : /^0\s(\d{1,2}\s){2}\?\s(\*\s)\d{1,2}$/, // "0 - - ? * -"
        "mes"  : /^0\s(\d{1,2}\s){3}\*\s\?$/,            // "0 - - - * ?"
        "ano"   : /^0\s(\d{1,2}\s){4}\?\s\*$/             // "0 - - - - ? *"
    };

    // ------------------ internal functions ---------------
    function defined(obj) {
        if (typeof obj == "undefined") { return false; }
        else { return true; }
    }

    function undefinedOrObject(obj) {
        return (!defined(obj) || typeof obj == "object")
    }

    function getCronType(cron_str, opts) {
        // if customValues defined, check for matches there first
        if (defined(opts.customValues)) {
            for (key in opts.customValues) {
                if (cron_str == opts.customValues[key]) { return key; }
            }
        }

        // check format of initial cron value
        var valid_cron = /^0\s(0\/1|\d{1,2})\s(0\/1|\d{1,2}|\*)\s(\d{1,2}|\*|\?)\s(\d{1,2}|\*)\s(\d{1,2}|\?)(\s\*)?$/
        if (typeof cron_str != "string" || !valid_cron.test(cron_str)) {
            $.error("cron: invalid initial value");
            return undefined;
        }

        // check actual cron values
        var d = cron_str.split(" ");

        d = d.splice(1, d.length - 1);      // remove first 0

        //            mm, hh, DD, MM, DOW
        var minval = [ 0,  0,  1,  1,  1];
        var maxval = [59, 23, 31, 12,  7];
        for (var i = 0; i < d.length; i++) {
            if (d[i] == "*") continue;
            if (/^0\/1$/.test(d[i])) continue;
            if (d[i] == "?") continue;

            var v = parseInt(d[i]);
            if (defined(v) && v <= maxval[i] && v >= minval[i]) continue;

            $.error("cron: invalid value found (col "+(i+1)+") in " + o.initial);
            return undefined;
        }

        // determine combination
        for (var t in combinations) {
            if (combinations[t].test(cron_str)) {
                return t;
            }
        }

        // unknown combination
        $.error("cron: valid but unsupported cron format. sorry.");
        return undefined;
    }

    function hasError(c, o) {
        if (!defined(getCronType(o.initial, o))) { return true; }
        if (!undefinedOrObject(o.customValues)) { return true; }

        // ensure that customValues keys do not coincide with existing fields
        if (defined(o.customValues)) {
            for (key in o.customValues) {
                if (combinations.hasOwnProperty(key)) {
                    $.error("cron: reserved keyword '" + key +
                            "' should not be used as customValues key.");
                    return true;
                }
            }
        }

        return false;
    }

    function getCurrentValue(c) {
        var b = c.data("block");
        var min = hora = dia = mes = dow = "*";
        var selectedPeriod = b["period"].find("select").val();
        switch (selectedPeriod) {
            case "minuto":
                return ["0", "0/1", "*", "*", "*", "?"].join(" ");
                break;

            case "hora":
                min = b["mins"].find("select").val();
                return ["0", min, "0/1", "*", "*", "?"].join(" ");
                break;

            case "dia":
                min  = b["time"].find("select.cron-time-min").val();
                hora = b["time"].find("select.cron-time-hora").val();
                return ["0", min, hora, "*", "*", "?"].join(" ");
                break;

            case "semana":
                min  = b["time"].find("select.cron-time-min").val();
                hora = b["time"].find("select.cron-time-hora").val();
                dow  =  b["dow"].find("select").val();
                return ["0", min, hora, "?", "*", dow].join(" ");
                break;

            case "mes":
                min  = b["time"].find("select.cron-time-min").val();
                hora = b["time"].find("select.cron-time-hora").val();
                dia  = b["dom"].find("select").val();
                return ["0", min, hora, dia, "*", "?"].join(" ");
                break;

            case "ano":
                min  = b["time"].find("select.cron-time-min").val();
                hora = b["time"].find("select.cron-time-hora").val();
                dia  = b["dom"].find("select").val();
                mes = b["mes"].find("select").val();
                return ["0", min, hora, dia, mes, "?", "*"].join(" ");
                break;

            default:
                // we assume this only happens when customValues is set
                return selectedPeriod;
        }

    }

    // -------------------  PUBLIC METHODS -----------------

    var methods = {
        init : function(opts) {

            // init options
            var options = opts ? opts : {}; /* default to empty obj */
            var o = $.extend([], defaults, options);
            var eo = $.extend({}, defaults.effectOpts, options.effectOpts);
            $.extend(o, {
                minutoOpts     : $.extend({}, defaults.minutoOpts, eo, options.minutoOpts),
                domOpts        : $.extend({}, defaults.domOpts, eo, options.domOpts),
                mesOpts      : $.extend({}, defaults.mesOpts, eo, options.mesOpts),
                dowOpts        : $.extend({}, defaults.dowOpts, eo, options.dowOpts),
                timeHourOpts   : $.extend({}, defaults.timeHourOpts, eo, options.timeHourOpts),
                timeMinuteOpts : $.extend({}, defaults.timeMinuteOpts, eo, options.timeMinuteOpts)
            });

            // error checking
            if (hasError(this, o)) { return this; }

            // ---- define select boxes in the right order -----

            var block = [], custom_periods = "", cv = o.customValues;
            if (defined(cv)) { // prepend custom values if specified
                for (var key in cv) {
                    custom_periods += "<option value='" + cv[key] + "'>" + key + "</option>\n";
                }
            }

            block["period"] = $("<span class='cron-period'>"
                    + "Cada <select name='cron-period'>" + custom_periods
                    + str_opt_period + "</select> </span>")
                .appendTo(this)
                .data("root", this);

            var select = block["period"].find("select");
            select.bind("change.cron", event_handlers.periodChanged)
                  .data("root", this);
            if (o.useGentleSelect) select.gentleSelect(eo);

            block["dom"] = $("<span class='cron-block cron-block-dom'>"
                    + " los <select name='cron-dom'>" + str_opt_dom
                    + "</select> </span>")
                .appendTo(this)
                .data("root", this);

            select = block["dom"].find("select").data("root", this);
            if (o.useGentleSelect) select.gentleSelect(o.domOpts);

            block["mes"] = $("<span class='cron-block cron-block-mes'>"
                    + " de <select name='cron-mes'>" + str_opt_mes
                    + "</select> </span>")
                .appendTo(this)
                .data("root", this);

            select = block["mes"].find("select").data("root", this);
            if (o.useGentleSelect) select.gentleSelect(o.mesOpts);

            block["mins"] = $("<span class='cron-block cron-block-mins'>"
                    + " a las <select name='cron-mins'>" + str_opt_mih
                    + "</select> minutos pasada la hora </span>")
                .appendTo(this)
                .data("root", this);

            select = block["mins"].find("select").data("root", this);
            if (o.useGentleSelect) select.gentleSelect(o.minutoOpts);

            block["dow"] = $("<span class='cron-block cron-block-dow'>"
                    + " los <select name='cron-dow'>" + str_opt_dow
                    + "</select> </span>")
                .appendTo(this)
                .data("root", this);

            select = block["dow"].find("select").data("root", this);
            if (o.useGentleSelect) select.gentleSelect(o.dowOpts);

            block["time"] = $("<span class='cron-block cron-block-time'>"
                    + " a las <select name='cron-time-hora' class='cron-time-hora'>" + str_opt_hid
                    + "</select>:<select name='cron-time-min' class='cron-time-min'>" + str_opt_mih
                    + " </span>")
                .appendTo(this)
                .data("root", this);

            select = block["time"].find("select.cron-time-hora").data("root", this);
            if (o.useGentleSelect) select.gentleSelect(o.timeHourOpts);
            select = block["time"].find("select.cron-time-min").data("root", this);
            if (o.useGentleSelect) select.gentleSelect(o.timeMinuteOpts);

            block["controls"] = $("<span class='cron-controls'>&laquo; save "
                    + "<span class='cron-button cron-button-save'></span>"
                    + " </span>")
                .appendTo(this)
                .data("root", this)
                .find("span.cron-button-save")
                    .bind("click.cron", event_handlers.saveClicked)
                    .data("root", this)
                    .end();

            this.find("select").bind("change.cron-callback", event_handlers.somethingChanged);
            this.data("options", o).data("block", block); // store options and block pointer
            this.data("current_value", o.initial); // remember base value to detect changes

            return methods["value"].call(this, o.initial); // set initial value
        },

        value : function(cron_str) {
            // when no args, act as getter
            if (!cron_str) { return getCurrentValue(this); }

            var o = this.data('options');
            var block = this.data("block");
            var useGentleSelect = o.useGentleSelect;
            var t = getCronType(cron_str, o);

            if (!defined(t)) { return false; }

            if (defined(o.customValues) && o.customValues.hasOwnProperty(t)) {
                t = o.customValues[t];
            } else {
                var d = cron_str.split(" ");

                d = d.splice(1, d.length - 1);  // Remove first 0

                for (var i = 0; i < d.length; i++) {                // Remove non digits
                    if(/^0\/1$/.test(d[i])) { d[i] = undefined; }
                    if(d[i] === '?') { d[i] = undefined; }
                }

                var v = {
                    "mins"  : d[0],
                    "hora"  : d[1],
                    "dom"   : d[2],
                    "mes" : d[3],
                    "dow"   : d[4]
                };

                // update appropriate select boxes
                var targets = toDisplay[t];
                for (var i = 0; i < targets.length; i++) {
                    var tgt = targets[i];
                    if (tgt == "time") {
                        var btgt = block[tgt].find("select.cron-time-hora").val(v["hora"]);
                        if (useGentleSelect) btgt.gentleSelect("update");

                        btgt = block[tgt].find("select.cron-time-min").val(v["mins"]);
                        if (useGentleSelect) btgt.gentleSelect("update");
                    } else {;
                        var btgt = block[tgt].find("select").val(v[tgt]);
                        if (useGentleSelect) btgt.gentleSelect("update");
                    }
                }
            }

            // trigger change event
            var bp = block["period"].find("select").val(t);
            if (useGentleSelect) bp.gentleSelect("update");
            bp.trigger("change");

            return this;
        }

    };

    var event_handlers = {
        periodChanged : function() {
            var root = $(this).data("root");
            var block = root.data("block"),
                opt = root.data("options");
            var period = $(this).val();

            root.find("span.cron-block").hide(); // first, hide all blocks
            if (toDisplay.hasOwnProperty(period)) { // not custom value
                var b = toDisplay[$(this).val()];
                for (var i = 0; i < b.length; i++) {
                    block[b[i]].show();
                }
            }
        },

        somethingChanged : function() {
            root = $(this).data("root");
            // if AJAX url defined, show "save"/"reset" button
            if (defined(root.data("options").url_set)) {
                if (methods.value.call(root) != root.data("current_value")) { // if changed
                    root.addClass("cron-changed");
                    root.data("block")["controls"].fadeIn();
                } else { // values manually reverted
                    root.removeClass("cron-changed");
                    root.data("block")["controls"].fadeOut();
                }
            } else {
                root.data("block")["controls"].hide();
            }

            // chain in user defined event handler, if specified
            var oc = root.data("options").onChange;
            if (defined(oc) && $.isFunction(oc)) {
                oc.call(root);
            }
        },

        saveClicked : function() {
            var btn  = $(this);
            var root = btn.data("root");
            var cron_str = methods.value.call(root);

            if (btn.hasClass("cron-loading")) { return; } // in progress
            btn.addClass("cron-loading");

            $.ajax({
                type : "POST",
                url  : root.data("options").url_set,
                data : { "cron" : cron_str },
                success : function() {
                    root.data("current_value", cron_str);
                    btn.removeClass("cron-loading");
                    // data changed since "save" clicked?
                    if (cron_str == methods.value.call(root)) {
                        root.removeClass("cron-changed");
                        root.data("block").controls.fadeOut();
                    }
                },
                error : function() {
                    alert("An error occured when submitting your request. Try again?");
                    btn.removeClass("cron-loading");
                }
            });
        }
    };

    $.fn.cron = function(method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || ! method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error( 'Method ' +  method + ' does not exist on jQuery.cron' );
        }
    };

})(jQuery);
