angular.module('umbraco.services').factory('Our.Umbraco.DocTypeGridEditor.Services.DocTypeDialogService',
    function (dialogService, editorState) {
        return {
            open: function (options) {

                var o = $.extend({}, {
                    template: "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html",
                    show: true,
                    requireName: true,
                    modalClass: "umb-modal wide dtge-modal"
                }, options);

                // Launch the dialog
                dialogService.open(o);
            }
        };
    });
