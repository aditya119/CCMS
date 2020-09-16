-------------------------------------------------------------------------
-- PKG_CASE_PROCEEDINGS: Perform CRUD operations on CASE_PROCEEDINGS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_proceedings IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_case_proceedings (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        po_cursor               OUT sys_refcursor
    );

    PROCEDURE p_assign_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_assigned_user        IN case_proceedings.assigned_to%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    );

    PROCEDURE p_update_case_proceedings (
        pi_case_id              IN case_proceedings.case_id%type,
        pi_proceeding_date      IN case_proceedings.proceeding_date%type,
        pi_proceeding_decision  IN case_proceedings.proceeding_decision%type,
        pi_next_hearing_on      IN case_proceedings.next_hearing_on%type,
        pi_judgement_file       IN case_proceedings.judgement_file%type,
        pi_assigned_to          IN case_proceedings.assigned_to%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    );

    PROCEDURE p_delete_case_proceedings (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type
    );

END pkg_case_proceedings;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_case_proceedings IS
-------------------------------------------------------------------------
    PROCEDURE p_get_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                cac.case_proceeding_id,
                cac.actor_type_id,
                cac.actor_name,
                cac.actor_address,
                cac.actor_email,
                cac.actor_phone,
                cac.detail_file,
                att.filename,
                cac.deleted
            from
                case_proceedings cac,
                attachments att
            where   cac.detail_file = att.attachment_id
                and case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_case_proceedings - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_case_proceedings;
-------------------------------------------------------------------------
    PROCEDURE p_update_case_proceedings (
        pi_case_id          IN case_proceedings.case_id%type,
        pi_actor_type_id    IN case_proceedings.actor_type_id%type,
        pi_actor_name       IN case_proceedings.actor_name%type,
        pi_actor_address    IN case_proceedings.actor_address%type,
        pi_actor_email      IN case_proceedings.actor_email%type,
        pi_actor_phone      IN case_proceedings.actor_phone%type,
        pi_detail_file      IN case_proceedings.detail_file%type,
        pi_update_by        IN case_proceedings.last_update_by%type
    ) IS
    BEGIN
        update case_proceedings
        set actor_name = pi_actor_name,
            actor_address = pi_actor_address,
            actor_email = pi_actor_email,
            actor_phone = pi_actor_phone,
            detail_file = pi_detail_file,
            last_update_by = pi_update_by
        where case_id = pi_case_id
          and actor_type_id = pi_actor_type_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_case_proceedings - pi_case_id: ' || pi_case_id
                || '; pi_actor_type_id: ' || pi_actor_type_id
                || '; pi_actor_name: ' || pi_actor_name
                || '; pi_actor_address: ' || pi_actor_address
                || '; pi_actor_email: ' || pi_actor_email
                || '; pi_actor_phone: ' || pi_actor_phone
                || '; pi_detail_file: ' || pi_detail_file
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_update_case_proceedings;
-------------------------------------------------------------------------
    PROCEDURE p_delete_case_proceedings (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type
    ) IS
    BEGIN
        update case_proceedings
        set deleted = sysdate
        where case_proceeding_id = pi_case_proceeding_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case_proceedings - pi_case_proceeding_id: ' || pi_case_proceeding_id
                || chr(10) || sqlerrm);
    END p_delete_case_proceedings;
-------------------------------------------------------------------------
END pkg_case_proceedings;
/
-------------------------------------------------------------------------