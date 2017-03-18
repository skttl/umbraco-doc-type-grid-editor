angular.module('umbraco.services').factory('Our.Umbraco.DocTypeGridEditor.Services.DocTypeDialogService',
    function (dialogService, editorState, umbRequestHelper) {
        return {
            open: function (options) {

                var o = $.extend({}, {
                    template: umbRequestHelper.convertVirtualToAbsolutePath("~/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html"),
                    show: true,
                    requireName: true,
                }, options);

                // Launch the dialog
                dialogService.open(o);
            }
        };
    });