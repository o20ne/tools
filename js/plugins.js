// Avoid `console` errors in browsers that lack a console.
(function() {
    var method;
    var noop = function () {};
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());

// Placeholder fix
/* <![CDATA[ */
$(function () {
  var input = document.createElement("input");
  if (('placeholder' in input) == false) {
    $('[placeholder]').focus(function () {
      var i = $(this);
      if (i.val() == i.attr('placeholder')) {
        i.val('').removeClass('placeholder');
        if (i.hasClass('password')) {
          i.removeClass('password');
          this.type = 'password';
        }
      }
    }).blur(function () {
      var i = $(this);
      if (i.val() == '' || i.val() == i.attr('placeholder')) {
        if (this.type == 'password') {
          i.addClass('password');
          this.type = 'text';
        }
        i.addClass('placeholder').val(i.attr('placeholder'));
      }
    }).blur().parents('form').submit(function () {
      $(this).find('[placeholder]').each(function () {
        var i = $(this);
        if (i.val() == i.attr('placeholder'))
          i.val('');
      })
    });
  }
});
/* ]]> */

// indexOf for IE8
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;
        var from = Number(arguments[1]) || 0;
        from = (from < 0) ? Math.ceil(from) : Math.floor(from);
        if (from < 0) from += len;

        for (; from < len; from++) {
            if (from in this && this[from] === elt) return from;
        }
        return -1;
    };
}

// trim function for IE8
if (typeof String.prototype.trim !== 'function') {
  String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, '');
  }
}

// Maxlength and countdown function
$(document).ready(function () {

  $("textarea[maxlength]").each(function () {

    $(this).bind("keyup change", function (e) {

      var eleMaxLength = $(this).attr('maxlength');
      var currentLength = $(this).val().length;
      var counterElement = $(this).siblings('.character-limit');

      if (currentLength > eleMaxLength) {
        $(this).val($(this).val().slice(0, eleMaxLength));
      }

      currentLength = $(this).val().length;
      var counterText = (parseInt(eleMaxLength) - parseInt(currentLength)) + " characters";
      $(counterElement).text(counterText);
    });

  });
});
// Place any jQuery/helper plugins in here.
