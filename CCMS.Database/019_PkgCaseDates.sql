-------------------------------------------------------------------------
-- PKG_CASE_DATES: Perform CRUD operations on CASE_DATES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_dates IS
-------------------------------------------------------------------------
    PROCEDURE p_get_case_dates (
        pi_case_date_id IN case_dates.case_date_id%type,
        po_cursor       OUT sys_refcursor
    );

    PROCEDURE p_add_case_dates (
        pi_case_id              IN case_dates.case_id%type,
        pi_case_filed_on        IN case_dates.case_filed_on%type,
        pi_notice_received_on   IN case_dates.notice_received_on%type,
        pi_first_hearing_on     IN case_dates.first_hearing_on%type,
        pi_update_by            IN case_dates.last_update_by%type
    );

    
    PROCEDURE p_update_case_dates (
        pi_case_date_id         IN case_dates.case_date_id%type,
        pi_case_filed_on        IN case_dates.case_filed_on%type,
        pi_notice_received_on   IN case_dates.notice_received_on%type,
        pi_first_hearing_on     IN case_dates.first_hearing_on%type,
        pi_update_by            IN case_dates.last_update_by%type
    );

    PROCEDURE p_delete_case_dates (
        pi_case_id  IN case_dates.case_id%type
    );

END pkg_lawyers;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------