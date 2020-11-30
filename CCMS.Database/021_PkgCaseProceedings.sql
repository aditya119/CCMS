-------------------------------------------------------------------------
-- PKG_CASE_PROCEEDINGS: Perform CRUD operations on CASE_PROCEEDINGS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_proceedings IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_proceedings (
        pi_case_id  IN case_proceedings.case_id%type,
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_get_assigned_proceedings (
        pi_user_id  IN case_proceedings.assigned_to%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_get_proceeding_details (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        po_cursor               OUT sys_refcursor
    );

    PROCEDURE p_assign_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_assign_to            IN case_proceedings.assigned_to%type,
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

    PROCEDURE p_delete_case_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
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
                case_proceeding_id,
                proceeding_date,
                proceeding_decision,
                next_hearing_on,
                judgement_file,
                assigned_to
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
    PROCEDURE p_get_assigned_proceedings (
        pi_user_id  IN case_proceedings.assigned_to%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                cc.case_id,
                cp.case_proceeding_id,
                cc.case_number,
                cc.appeal_number,
                cp.proceeding_date,
                pd.proceeding_decision_name case_status,
                cp.next_hearing_on,
                au.user_fullname || ' (' || au.user_email || ')' assigned_to
            from
                court_cases cc, 
                case_proceedings cp,
                proceeding_decisions pd,
                app_users au
            where   cc.deleted is null
                and cp.deleted is null
                and pd.deleted is null
                and cc.case_id = cp.case_id
                and cp.assigned_to = au.user_id
                and cc.case_status = pd.proceeding_decision_id
                and cp.proceeding_decision = 0 -- Pending
                and (-1 = pi_user_id or cp.assigned_to = pi_user_id)
            order by
                cp.proceeding_date;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_assigned_proceedings - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END p_get_assigned_proceedings;
-------------------------------------------------------------------------
    PROCEDURE p_get_proceeding_details (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        po_cursor               OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_proceeding_id,
                proceeding_date,
                proceeding_decision,
                next_hearing_on,
                judgement_file,
                assigned_to
            from
                case_proceedings
            where   case_proceeding_id = pi_case_proceeding_id
                and deleted is null;
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
        pi_assign_to            IN case_proceedings.assigned_to%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    ) IS
    BEGIN
        update case_proceedings
        set assigned_to = pi_assign_to,
            last_update_by = pi_update_by
        where case_proceeding_id = pi_case_proceeding_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_assign_proceeding - pi_case_proceeding_id: ' || pi_case_proceeding_id
                || '; pi_assign_to: ' || pi_assign_to
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
    PROCEDURE p_delete_case_proceeding (
        pi_case_proceeding_id   IN case_proceedings.case_proceeding_id%type,
        pi_update_by            IN case_proceedings.last_update_by%type
    ) IS
    BEGIN
        update case_proceedings
        set deleted = sysdate,
            last_update_by = pi_update_by
        where case_proceeding_id = pi_case_proceeding_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case_proceeding - pi_case_proceeding_id: ' || pi_case_proceeding_id
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_delete_case_proceeding;
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