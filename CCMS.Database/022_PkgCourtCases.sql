-------------------------------------------------------------------------
-- PKG_COURT_CASES: Perform CRUD operations on COURT_CASES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_court_cases IS
-------------------------------------------------------------------------
    PROCEDURE p_get_assigned_cases (
        pi_user_id  IN court_cases.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_get_case_details (
        pi_case_id  IN court_cases.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_exists_case_number (
        pi_case_number      IN court_cases.case_number%type,
        po_case_id          OUT court_cases.case_id%type,
        po_appeal_number    OUT court_cases.appeal_number%type
    );

    PROCEDURE p_exists_case_id (
        pi_case_id          IN court_cases.case_id%type,
        po_case_number      OUT court_cases.case_number%type,
        po_appeal_number    OUT court_cases.appeal_number%type
    );

    PROCEDURE p_add_new_case (
        pi_case_number  IN court_cases.case_number%type,
        pi_case_type_id IN court_cases.case_type_id%type,
        pi_court_id     IN court_cases.court_id%type,
        pi_location_id  IN court_cases.location_id%type,
        pi_lawyer_id    IN court_cases.lawyer_id%type,
        pi_action_by    IN court_cases.created_by%type
    );

    PROCEDURE p_update_court_case (
        pi_case_number  IN court_cases.case_number%type,
        pi_case_type_id IN court_cases.case_type_id%type,
        pi_court_id     IN court_cases.court_id%type,
        pi_location_id  IN court_cases.location_id%type,
        pi_lawyer_id    IN court_cases.lawyer_id%type,
        pi_update_by    IN court_cases.last_update_by%type
    );

    PROCEDURE p_delete_court_case (
        pi_case_id  IN court_cases.case_id%type
    );

END pkg_court_cases;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_court_cases IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_court_cases (
        pi_case_id  IN court_cases.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                case_id,
                proceeding_date,
                proceeding_decision,
                next_hearing_on,
                judgement_file,
                assigned_to,
                last_update_by
            from
                court_cases
            where   deleted is null
                and case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_court_cases - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_all_court_cases;
-------------------------------------------------------------------------
    PROCEDURE p_get_case_details (
        pi_case_id  IN court_cases.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                case_number,
                appeal_number,
                case_type_id,
                court_id,
                location_id,
                lawyer_id,
                case_status,
                deleted
            from
                court_cases
            where
                case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_case_details - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_case_details;
-------------------------------------------------------------------------
    PROCEDURE p_assign_proceeding (
        pi_case_id   IN court_cases.case_id%type,
        pi_assigned_user        IN court_cases.assigned_to%type,
        pi_update_by            IN court_cases.last_update_by%type
    ) IS
    BEGIN
        update court_cases
        set assigned_to = pi_assigned_user,
            last_update_by = pi_update_by
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_assign_proceeding - pi_case_id: ' || pi_case_id
                || '; pi_assigned_user: ' || pi_assigned_user
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_assign_proceeding;
-------------------------------------------------------------------------
    PROCEDURE p_update_court_case (
        pi_case_id   IN court_cases.case_id%type,
        pi_proceeding_date      IN court_cases.proceeding_date%type,
        pi_proceeding_decision  IN court_cases.proceeding_decision%type,
        pi_next_hearing_on      IN court_cases.next_hearing_on%type,
        pi_judgement_file       IN court_cases.judgement_file%type,
        pi_update_by            IN court_cases.last_update_by%type
    ) IS
    BEGIN
        update court_cases
        set proceeding_date = pi_proceeding_date,
            proceeding_decision = pi_proceeding_decision,
            next_hearing_on = pi_next_hearing_on,
            judgement_file = pi_judgement_file,
            last_update_by = pi_update_by
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_court_case - pi_case_id: ' || pi_case_id
                || '; pi_proceeding_date: ' || pi_proceeding_date
                || '; pi_proceeding_decision: ' || pi_proceeding_decision
                || '; pi_next_hearing_on: ' || pi_next_hearing_on
                || '; pi_judgement_file: ' || pi_judgement_file
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_update_court_case;
-------------------------------------------------------------------------
    PROCEDURE p_delete_court_case (
        pi_case_id  IN court_cases.case_id%type
    ) IS
    BEGIN
        update court_cases
        set deleted = sysdate
        where case_id = pi_case_id;
        
        pkg_case_dates.p_delete_case_dates(pi_case_id);
        
        pkg_case_actors.p_delete_case_actors(pi_case_id);
        
        pkg_case_proceedings.p_delete_case_proceedings(pi_case_id);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_court_case - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_delete_court_case;
-------------------------------------------------------------------------
END pkg_court_cases;
/
-------------------------------------------------------------------------