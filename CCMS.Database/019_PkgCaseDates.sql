-------------------------------------------------------------------------
-- PKG_CASE_DATES: Perform CRUD operations on CASE_DATES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_dates IS
-------------------------------------------------------------------------
    PROCEDURE p_get_case_dates (
        pi_case_id  IN case_dates.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_update_case_dates (
        pi_case_id              IN case_dates.case_id%type,
        pi_case_filed_on        IN case_dates.case_filed_on%type,
        pi_notice_received_on   IN case_dates.notice_received_on%type,
        pi_first_hearing_on     IN case_dates.first_hearing_on%type,
        pi_update_by            IN case_dates.last_update_by%type
    );

    PROCEDURE p_delete_case_dates (
        pi_case_id  IN case_dates.case_id%type
    );

END pkg_case_dates;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_case_dates IS
-------------------------------------------------------------------------
    PROCEDURE p_get_case_dates (
        pi_case_id  IN case_dates.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                case_filed_on,
                notice_received_on,
                first_hearing_on
            from
                case_dates
            where   case_id = pi_case_id
                and deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_case_dates - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_case_dates;
-------------------------------------------------------------------------
    PROCEDURE p_update_case_dates (
        pi_case_id              IN case_dates.case_id%type,
        pi_case_filed_on        IN case_dates.case_filed_on%type,
        pi_notice_received_on   IN case_dates.notice_received_on%type,
        pi_first_hearing_on     IN case_dates.first_hearing_on%type,
        pi_update_by            IN case_dates.last_update_by%type
    ) IS
    BEGIN
        update case_dates
        set case_filed_on = pi_case_filed_on,
            notice_received_on = pi_notice_received_on,
            first_hearing_on = pi_first_hearing_on,
            last_update_by = pi_update_by
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_case_dates - pi_case_id: ' || pi_case_id
                || '; pi_case_filed_on: ' || pi_case_filed_on
                || '; pi_notice_received_on: ' || pi_notice_received_on
                || '; pi_first_hearing_on: ' || pi_first_hearing_on
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_update_case_dates;
-------------------------------------------------------------------------
    PROCEDURE p_delete_case_dates (
        pi_case_id  IN case_dates.case_id%type
    ) IS
    BEGIN
        update case_dates
        set deleted = sysdate
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case_dates - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_delete_case_dates;
-------------------------------------------------------------------------
END pkg_case_dates;
/
-------------------------------------------------------------------------