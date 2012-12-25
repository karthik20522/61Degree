var items = [];

var JQUERY4U = {};
JQUERY4U.UTIL = {
    formatVarString: function () {
        var args = [].slice.call(arguments);
        if (this.toString() != '[object Object]') {
            args.unshift(this.toString());
        }

        var pattern = new RegExp('{([1-' + args.length + '])}', 'g');
        return String(args[0]).replace(pattern, function (match, index) { return args[index]; });
    }
}

jQuery(document).ready(function ($) {

    if (!(/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent))) {
        $(".taphere").hide();
    }
    $(".block").each(function () {
        items.push($(this).attr("id"));
    });

    items.reverse();

    Tipped.create('.block', function (element) {
        var image = $(element).data('image');
        var title = $(element).data('title');
        var url = $(element).data('url');
        var time = $(element).data('time');
        var publisher = $(element).data('publisher');

        var addToRecent = JQUERY4U.UTIL.formatVarString("<div class='preview'>" +
            "<a href='{4}' target='_blank'><img class='previewImage' src='{1}' alt='{2}' /></a>" +
            "<span>{2} - <a class='prevPublisher' href='{4}' target='_blank'>{3}</a></span></div>", image, title, publisher, url);
        $(element).css('border', '1px dashed black');
        return JQUERY4U.UTIL.formatVarString(addToRecent);
    },
    {
        //skin: 'white',
        hook: { target: 'topleft', tooltip: 'bottomleft' },       
        closeButton: true,
        showDelay: 250,
        hideOthers: true,
        showOn: ['click', 'mouseover']
    });

    $('.taphere').addSwipeEvents().
        bind('doubletap', function (evt, touch) {
            var id = items.pop();
            $("#" + id).focus().click();
    });
});
 