-------------------------------------------------------------------------
-- PKG_CASE_PROCEEDINGS: Perform CRUD operations on CASE_PROCEEDINGS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_proceedings IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_get_proceeding_details (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        po_cursor               OUT sys_refcursor
    );

    PROCEDURE p_assign_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_assigned_user        IN case_proceedings.assigned_to%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    );

    PROCEDURE p_update_case_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_proceeding_date      IN case_proceedings.proceeding_date%type,
        pi_proceeding_decision  IN case_proceedings.proceeding_decision%type,
        pi_next_hearing_on      IN case_proceedings.next_hearing_on%type,
        pi_judgement_file       IN case_proceedings.judgement_file%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    );

    PROCEDURE p_delete_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type
    );

END pkg_case_proceedings;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_case_proceedings IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                case_proceeding_id,
                proceeding_date,
                proceeding_decision,
                next_hearing_on,
                judgement_file,
                assigned_to,
                last_update_by
            from
                case_proceedings
            where   deleted is null
                and case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_case_proceedings - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_all_case_proceedings;
-------------------------------------------------------------------------
    PROCEDURE p_get_proceeding_details (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        po_cursor               OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                case_proceeding_id,
                proceeding_date,
                proceeding_decision,
                next_hearing_on,
                judgement_file,
                assigned_to,
                last_update_by
            from
                case_proceedings
            where
                case_proceeding_id = pi_case_proceeding_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_proceeding_details - pi_case_proceeding_id: ' || pi_case_proceeding_id
                || chr(10) || sqlerrm);
    END p_get_proceeding_details;
-------------------------------------------------------------------------
    PROCEDURE p_assign_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_assigned_user        IN case_proceedings.assigned_to%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    ) IS
    BEGIN
        update case_proceedings
        set assigned_to = pi_assigned_user,
            last_update_by = pi_update_by
        where case_proceeding_id = pi_case_proceeding_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_assign_proceeding - pi_case_proceeding_id: ' || pi_case_proceeding_id
                || '; pi_assigned_user: ' || pi_assigned_user
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_assign_proceeding;
-------------------------------------------------------------------------
    PROCEDURE p_update_case_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_proceeding_date      IN case_proceedings.proceeding_date%type,
        pi_proceeding_decision  IN case_proceedings.proceeding_decision%type,
        pi_next_hearing_on      IN case_proceedings.next_hearing_on%type,
        pi_judgement_file       IN case_proceedings.judgement_file%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    ) IS
    BEGIN
        update case_proceedings
        set proceeding_date = pi_proceeding_date,
            proceeding_decision = pi_proceeding_decision,
            next_hearing_on = pi_next_hearing_on,
            judgement_file = pi_judgement_file,
            last_update_by = pi_update_by
        where case_proceeding_id = pi_case_proceeding_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_case_proceeding - pi_case_proceeding_id: ' || pi_case_proceeding_id
                || '; pi_proceeding_date: ' || pi_proceeding_date
                || '; pi_proceeding_decision: ' || pi_proceeding_decision
                || '; pi_next_hearing_on: ' || pi_next_hearing_on
                || '; pi_judgement_file: ' || pi_judgement_file
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_update_case_proceeding;
-------------------------------------------------------------------------
    PROCEDURE p_delete_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type
    ) IS
    BEGIN
        update case_proceedings
        set deleted = sysdate
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case_proceedings - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_delete_case_proceedings;
-------------------------------------------------------------------------
END pkg_case_proceedings;
/
-------------------------------------------------------------------------