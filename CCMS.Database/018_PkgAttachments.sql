-------------------------------------------------------------------------
-- PKG_ATTACHMENTS: Perform CRUD operations on ATTACHMENTS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_attachments IS
-------------------------------------------------------------------------
    PROCEDURE p_get_attachment_details (
        pi_attachment_id    IN attachments.attachment_id%type,
        po_cursor           OUT sys_refcursor
    );

    PROCEDURE p_download_attachment (
        pi_attachment_id    IN attachments.attachment_id%type,
        po_cursor           OUT sys_refcursor
    );

    PROCEDURE p_delete_attachment (
        pi_attachment_id  IN attachments.attachment_id%type,
        pi_update_by      IN attachments.last_update_by%type
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
                filename,
                content_type
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
    PROCEDURE p_download_attachment (
        pi_attachment_id    IN attachments.attachment_id%type,
        po_cursor           OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                attachment_file
            from
                attachments
            where
                attachment_id = pi_attachment_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_download_attachment - pi_attachment_id: ' || pi_attachment_id
                || chr(10) || sqlerrm);
    END p_download_attachment;
-------------------------------------------------------------------------
    PROCEDURE p_delete_attachment (
        pi_attachment_id  IN attachments.attachment_id%type,
        pi_update_by      IN attachments.last_update_by%type
    ) IS
    BEGIN
        if pi_attachment_id < 1 then
            raise_application_error(-20002, 'pi_attachment_id, ' || pi_attachment_id || ', < 1');
        end if;

        -- to audit log deletion
        update attachments
        set last_update_by = pi_update_by,
            deleted = sysdate
        where attachment_id = pi_attachment_id;

        delete from attachments
        where attachment_id = pi_attachment_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_attachment - pi_attachment_id: ' || pi_attachment_id
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_delete_attachment;
-------------------------------------------------------------------------
END pkg_attachments;
/
-------------------------------------------------------------------------