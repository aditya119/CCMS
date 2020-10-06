-------------------------------------------------------------------------
-- PKG_ATTACHMENTS: Perform CRUD operations on ATTACHMENTS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_attachments IS
-------------------------------------------------------------------------
    PROCEDURE p_get_attachment_details (
        pi_attachment_id    IN attachments.attachment_id%type,
        po_cursor           OUT sys_refcursor
    );

    PROCEDURE p_create_new_attachment (
        pi_filename         IN attachments.filename%type,
        po_attachment_id    OUT attachments.attachment_id%type
    );

    PROCEDURE p_update_attachment (
        pi_attachment_id    IN attachments.attachment_id%type,
        pi_filename         IN attachments.filename%type
    );

    PROCEDURE p_delete_attachment (
        pi_attachment_id  IN attachments.attachment_id%type
    );

END pkg_attachments;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_attachments IS
-------------------------------------------------------------------------
    PROCEDURE p_get_attachment_details (
        pi_attachment_id  IN attachments.attachment_id%type,
        po_cursor       OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                attachment_id,
                filename
            from
                attachments
            where
                attachment_id = pi_attachment_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_attachment_details - pi_attachment_id: ' || pi_attachment_id
                || chr(10) || sqlerrm);
    END p_get_attachment_details;
-------------------------------------------------------------------------
    PROCEDURE p_create_new_attachment (
        pi_filename         IN attachments.filename%type,
        po_attachment_id    OUT attachments.attachment_id%type
    ) IS
        v_deleted   attachments.deleted%type;
    BEGIN
        insert into attachments (
            filename
        ) values (
            pi_filename
        )
        returning
            attachment_id
        into
            po_attachment_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_new_attachment - pi_filename: ' || pi_filename
                || chr(10) || sqlerrm);
    END p_create_new_attachment;
-------------------------------------------------------------------------
    PROCEDURE p_update_attachment (
        pi_attachment_id    IN attachments.attachment_id%type,
        pi_filename         IN attachments.filename%type
    ) IS
    BEGIN
        update attachments
        set filename = pi_filename,
            deleted = null
        where attachment_id = pi_attachment_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_attachment - pi_attachment_id: ' || pi_attachment_id
                || '; pi_filename:' || pi_filename
                || chr(10) || sqlerrm);
    END p_update_attachment;
-------------------------------------------------------------------------
    PROCEDURE p_delete_attachment (
        pi_attachment_id  IN attachments.attachment_id%type
    ) IS
    BEGIN
        update attachments
        set deleted = sysdate
        where attachment_id = pi_attachment_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_attachment - pi_attachment_id: ' || pi_attachment_id
                || chr(10) || sqlerrm);
    END p_delete_attachment;
-------------------------------------------------------------------------
END pkg_attachments;
/
-------------------------------------------------------------------------