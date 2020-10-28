importScripts('~/Scripts/momentjs.js');
function hi(id) {
    var a = $("#empnow option:selected").val();
    $("#name").val(a);
    $("#nameval").val($("#name option:selected").text());
    $("#nameval").html($("#name option:selected").text());
    $("#pos").val(a);
    $("#posval").val($("#pos option:selected").text());
    $("#posval").html($("#pos option:selected").text());
    $("#gra").val(a);
    $("#graval").val($("#gra option:selected").text());
    $("#graval").html($("#gra option:selected").text());
    $("#bac").val(a);
    $("#bacval").val($("#bac option:selected").text());
    $("#bacval").html($("#bac option:selected").text());
    $("#hou").val(a);
    $("#houval").val($("#hou option:selected").text());
    $("#houval").html($("#hou option:selected").text());
    $("#gro").val(a);
    $("#groval").val($("#gro option:selected").text());
    $("#groval").html($("#gro option:selected").text());
    $("#unp").val(a);
    $("#unpval").val($("#unp option:selected").text());
    $("#unpval").html($("#unp option:selected").text());
    $("#joi").val(a);
    $("#abs").val(a);
    $("#absval").val($("#abs option:selected").text());
    $("#absval").html($("#abs option:selected").text());
    var sno1 = parseInt($("#status option:selected").val());
    var from = $("#joi option:selected").text();
    var n = from.indexOf(" ");
    from = from.substring(0, n != -1 ? n : s.length);
    $("#joival").val(from);
    $("#joival").html(from);
    $("#joival2").val(from);
    $("#joival2").html(from);
    var dajo = new Date(from);
    var dala = new Date($("#datepicker").val());
    var unpd = 0;
    if ($("#unpval").val() != "") {
        unpd = parseInt($("#unpval").val());
    }
    var abs = 0;
    if ($("#absval").val() != "") {
        abs = parseInt($("#absval").val());
    }
    var bac = parseInt($("#bacval").val());
    var dwr = bac * 12 / 365;
    $("#bacvalpd").val(dwr.toFixed(2));
    $("#bacvalpd").html(dwr.toFixed(2));
    var ddd = dala.getTime() - dajo.getTime();
    var untw = (ddd / 1000 / 60 / 60 / 24) + 1;
    var tw = (((ddd / 1000 / 60 / 60 / 24) - unpd) - abs) + 1;
    var twmonth = (tw / (365 / 12)).toFixed(2);
    var twyrs = (twmonth / 12).toFixed(2);
    var untwmonth = (untw / (365 / 12)).toFixed(2);
    var untwyrs = (untwmonth / 12).toFixed(3);
    if (sno1 == 2) {
        if (tw > 364) {
            $("#ldval2").val(tw + " days " + twmonth + " months " + twyrs + " years");
            $("#ldval2").html((tw));
            $("#ldval3").val(untw + " days " + untwmonth + " months " + untwyrs + " years");
            $("#ldval3").html((untw));
            var eossum = 0;
            if (tw > 1824) {
                $("#eospwval").val(((tw) * 30 / 365 / 30).toFixed(2));
                $("#eospwval").html(((tw) * 30 / 365 / 30).toFixed(2));
                $("#td5val").val(1825 +
                    " days " +
                    (1825 / (365 / 12)).toFixed(2) +
                    " mounths " +
                    (1825 / (365 / 12) / 12).toFixed(2) +
                    " years");
                $("#td5val").html(1825 +
                    " days " +
                    (1825 / (365 / 12)).toFixed(2) +
                    " mounths " +
                    (1825 / (365 / 12) / 12).toFixed(2) +
                    " years");
                $("#tda5val").val(tw -
                    1825 +
                    " days " +
                    ((tw - 1825) / (365 / 12)).toFixed(2) +
                    " mounths " +
                    ((tw - 1825) / (365 / 12) / 12).toFixed(2) +
                    " years");
                $("#tda5val").html(tw -
                    1825 +
                    " days " +
                    ((tw - 1825) / (365 / 12)).toFixed(2) +
                    " mounths " +
                    ((tw - 1825) / (365 / 12) / 12).toFixed(2) +
                    " years");
                $("#eos5val").val(105 +
                    " days " +
                    (105 / (365 / 12)).toFixed(2) +
                    " mounths " +
                    (105 / (365 / 12) / 12).toFixed(2) +
                    " years");
                $("#eos5val").html(105 +
                    " days " +
                    (105 / (365 / 12)).toFixed(2) +
                    " mounths " +
                    (105 / (365 / 12) / 12).toFixed(2) +
                    " years");
                $("#value5").val((105 * dwr).toFixed(2));
                $("#value5").html((105 * dwr).toFixed(2));
                $("#after5eos").val(((tw - 1825) * 30 / 365).toFixed(2));
                $("#after5eos").html(((tw - 1825) * 30 / 365).toFixed(2));
                $("#after5eosv").val(((tw - 1825) * 30 / 365 * dwr).toFixed(2));
                $("#after5eosv").html(((tw - 1825) * 30 / 365 * dwr).toFixed(2));
                eossum = 105 + (((tw - 1825) * 30) / 365);
                $("#eostotal").val(eossum.toFixed(2));
                $("#eostotal").html(eossum.toFixed(2));
                $("#eostotalv").val((eossum * dwr).toFixed(2));
                $("#eostotalv").html((eossum * dwr).toFixed(2));
            } else {
                $("#eospwval").val(((tw * 21) / 365 / 30).toFixed(2));
                $("#eospwval").html(((tw * 21) / 365 / 30).toFixed(2));
                $("#td5val").val(tw);
                $("#td5val").html(tw);
                $("#tda5val").val(0);
                $("#tda5val").html(0);
                $("#eos5val").val((tw / 365) * 21);
                $("#eos5val").html((tw / 365) * 21);
                $("#value5").val((tw / 365) * 21 * dwr);
                $("#value5").html((tw / 365) * 21 * dwr);
                $("#after5eos").val(0);
                $("#after5eos").html(0);
                $("#after5eosv").val(0);
                $("#after5eosv").html(0);
                eossum = ((tw * 21) / 365);
                $("#eostotal").val(eossum.toFixed(2));
                $("#eostotal").html(eossum.toFixed(2));
                $("#eostotalv").val((eossum * dwr).toFixed(2));
                $("#eostotalv").html((eossum * dwr).toFixed(2));
            }
        } else {
            $("#eospwval").val(0);
            $("#eospwval").html(0);
            $("#ldval2").val(tw + " days " + twmonth + " months " + twyrs + " years");
            $("#ldval2").html((tw) );
            $("#ldval3").val(untw + " days " + untwmonth + " months " + untwyrs + " years");
            $("#ldval3").html((untw) );
            $("#td5val").val(0);
            $("#td5val").html(0);
            $("#tda5val").val(0);
            $("#tda5val").html(0);
            $("#eos5val").val(0);
            $("#eos5val").html(0);
            $("#value5").val(0);
            $("#value5").html(0);
            $("#after5eos").val(0);
            $("#after5eos").html(0);
            $("#eostotal").val(0);
            $("#eostotal").html(0);
            $("#eostotalv").val(0 * dwr);
            $("#eostotalv").html(0 * dwr);
        }
    } else {
        $("#eospwval").val(0);
        $("#eospwval").html(0);
        $("#ldval2").val(tw + " days " + twmonth + " months " + twyrs + " years");
        $("#ldval2").html((tw) );
        $("#ldval3").val(untw + " days " + untwmonth + " months " + untwyrs + " years");
        $("#ldval3").html((untw) );
        $("#td5val").val(0);
        $("#td5val").html(0);
        $("#tda5val").val(0);
        $("#tda5val").html(0);
        $("#eos5val").val(0);
        $("#eos5val").html(0);
        $("#value5").val(0);
        $("#value5").html(0);
        $("#after5eos").val(0);
        $("#after5eos").html(0);
        $("#eostotal").val(0);
        $("#eostotal").html(0);
        $("#eostotalv").val(0);
        $("#eostotalv").html(0);
        $("#eosl3").val(0);
        $("#eosl3").html(0);
        $("#eosl3v").val(0);
        $("#eosl3v").html(0);
        $("#eos3_5").val(0);
        $("#eos3_5").html(0);
        $("#eos3_5d").val(0);
        $("#eos3_5d").html(0);
        $("#eos5").val(0);
        $("#eos5").html(0);
        $("#eos5v").val(0);
        $("#eos5v").html(0);
        if (tw <= 1094 && tw >= 365) {

            $("#eosl3").val((tw) * 7 / 365);
            $("#eosl3").html((tw) * 7 / 365);
            $("#eosl3v").val((tw) * 7 / 365 * dwr);
            $("#eosl3v").html((tw) * 7 / 365 * dwr);
            $("#eostotal").val(((tw * 7) / 365).toFixed(2));
            $("#eostotal").html(((tw * 7) / 365).toFixed(2));
            $("#eostotalv").val(((tw) * 7 / 365 * dwr).toFixed(2));
            $("#eostotalv").html(((tw) * 7 / 365 * dwr).toFixed(2));
        } else if (tw >= 1094 && tw <= 1824) {
            $("#eosl3").val(1095 * 7 / 365);
            $("#eosl3").html(1095 * 7 / 365);
            $("#eosl3v").val(1095 * 7 / 365 * dwr);
            $("#eosl3v").html(1095 * 7 / 365 * dwr);
            $("#eos3_5").val((tw) * 14 / 365 * dwr);
            $("#eos3_5").html((tw) * 14 / 365 * dwr);
            $("#eos3_5d").val((tw) * 14 / 365);
            $("#eos3_5d").html((tw) * 14 / 365);
            $("#eostotalv").val(((tw) * 14 / 365 * dwr).toFixed(2));
            $("#eostotalv").html(((tw) * 14 / 365 * dwr).toFixed(2));
            $("#eostotal").val(((tw) * 14 / 365).toFixed(2));
            $("#eostotal").html(((tw) * 14 / 365).toFixed(2));
        } else if (tw > 1824) {
            $("#eosl3").val(1095 * 7 / 365);
            $("#eosl3").html(1095 * 7 / 365);
            $("#eosl3v").val(1095 * 7 / 365 * dwr);
            $("#eosl3v").html(1095 * 7 / 365 * dwr);
            $("#eos3_5").val(1825 * 14 / 365 * dwr);
            $("#eos3_5").html(1825 * 14 / 365 * dwr);
            $("#eos3_5d").val(1825 * 14 / 365);
            $("#eos3_5d").html(1825 * 14 / 365);
            $("#eos5").val((tw) * 21 / 365);
            $("#eos5").html((tw) * 21 / 365);
            $("#eos5v").val((tw) * 21 / 365 * dwr);
            $("#eos5v").html((tw) * 21 / 365 * dwr);
            $("#eostotal").val(((tw) * 21 / 365).toFixed(2));
            $("#eostotal").html(((tw) * 21 / 365).toFixed(2));
            $("#eostotalv").val(((tw) * 21 / 365 * dwr).toFixed(2));
            $("#eostotalv").html(((tw) * 21 / 365 * dwr).toFixed(2));
        } else {

            $("#eospwval").val(0);
            $("#eospwval").html(0);
            $("#ldval2").val(tw + " days " + twmonth + " months " + twyrs + " years");
            $("#ldval2").html((tw) );
            $("#ldval3").val(untw + " days " + untwmonth + " months " + untwyrs + " years");
            $("#ldval3").html((untw) );
            $("#td5val").val(0);
            $("#td5val").html(0);
            $("#tda5val").val(0);
            $("#tda5val").html(0);
            $("#eos5val").val(0);
            $("#eos5val").html(0);
            $("#value5").val(0);
            $("#value5").html(0);
            $("#after5eos").val(0);
            $("#after5eos").html(0);
            $("#eostotal").val(0);
            $("#eostotal").html(0);
            $("#eostotalv").val(0 * dwr);
            $("#eostotalv").html(0 * dwr);
            $("#eosl3").val(0);
            $("#eosl3").html(0);
            $("#eosl3v").val(0);
            $("#eosl3v").html(0);
            $("#eos3_5").val(0);
            $("#eos3_5").html(0);
            $("#eos3_5d").val(0);
            $("#eos3_5d").html(0);
            $("#eos5").val(0);
            $("#eos5").html(0);
            $("#eos5v").val(0);
            $("#eos5v").html(0);
        }
    }
}

function humanise(diff) {
    // The string we're working with to create the representation
    var str = '';
    // Map lengths of `diff` to different time periods
    var values = [[' year', 365], [' month', 30], [' day', 1]];
    // Iterate over the values...
    for (var i = 0; i < values.length; i++) {
        var amount = Math.floor(diff / values[i][1]);
        // ... and find the largest time value that fits into the diff
        if (amount >= 1) {
            // If we match, add to the string ('s' is for pluralization)
            str += amount + values[i][0] + (amount > 1 ? 's' : '') + ' ';

            // and subtract from the diff
            diff -= amount * values[i][1];
        }
    }
    return str;
}