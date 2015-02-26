angular.module('umbraco.services').factory('Our.Umbraco.DocTypeGridEditor.Services.DocTypeDialogService',
    function (dialogService, editorState) {
        return {
            open: function (options) {

                var currentEditorState = editorState.current;
                var callback = function () {
                    // We create a new editor state in the dialog,
                    // so be sure to set the previous one back 
                    // when we are done.
                    editorState.set(currentEditorState);
                };

                var o = $.extend({}, {
                    template: "/App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html",
                    show: true,
                    requireName: true,
                }, options);


                // Wrap callbacks and reset the editor state
                if ("callback" in o) {
                    var oldCallback = o.callback;
                    o.callback = function (data) {
                        oldCallback(data);
                        callback(data);
                    };
                } else {
                    o.callback = callback;
                }

                if ("closeCallback" in o) {
                    var oldCloseCallback = o.closeCallback;
                    o.closeCallback = function (data) {
                        oldCloseCallback(data);
                        callback(data);
                    };
                } else {
                    o.closeCallback = callback;
                }

                // Launch the dialog
                dialogService.open(o);
            }
        };
    });