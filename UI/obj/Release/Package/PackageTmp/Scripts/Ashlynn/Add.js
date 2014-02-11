/*global $, ASHLYNN */

ASHLYNN.namespace("add");

ASHLYNN.add = (function () {
    "use strict";

    var selector,
        getFileName,
        addFileSelector,
        toggleDelete,
        removeFile,
        ready;

    selector = {
        addAttachmentButton: "#add-attachment-button",
        attachmentsDiv: "#attachments-div",
        removeButton: "._remove-button",
        saveButton: "#save-button",
        deleteAttachmentButton: "._delete-attachment-button",
        cancelButton: "#cancel-button",
        div: "div",
        span: "span",
        anchor: "a",
        hiddenField: "input:hidden",
        label: "label"
    };

    getFileName = (function () {
        var number = 0;
        return function () {
            number += 1;
            return "File" + number;
        };
    }());

    addFileSelector = function () {
        var $fileUpload = $('<div><input type="file" name="' + getFileName() + '" /><button class="_remove-button">Remove</button></div>');
        $(selector.attachmentsDiv).append($fileUpload);
        $(selector.removeButton, $fileUpload).button();
    };

    toggleDelete = function () {
        var $deleteButton = $(this),
            $span = $(this).closest(selector.span),
            id = ASHLYNN.common.getData($span, "attachment-id"),
            $hiddenField = $(selector.hiddenField, $span),
            $link = $(selector.anchor, $span),
            $label = $(selector.label, $span);
        
        if ($hiddenField.length === 0) {
            $span.append('<input name="attachments" type="hidden" value="' + id + '" />');
            $deleteButton.text("Delete");
            $link.show();
            $label.hide();
        } else {
            $hiddenField.remove();
            $deleteButton.text("Undo");
            $link.hide();
            $label.show();
        }
    };

    removeFile = function () {
        var $div = $(this).closest(selector.div);
        $div.remove();
    };

    ready = function () {
        $(selector.addAttachmentButton).button().click(addFileSelector);
        $(selector.attachmentsDiv).on("click", selector.removeButton, removeFile);
        $(selector.saveButton).button();
        $(selector.cancelButton).button();
        $(selector.deleteAttachmentButton).button().click(toggleDelete);
    };

    return {
        ready: ready
    };
}());

