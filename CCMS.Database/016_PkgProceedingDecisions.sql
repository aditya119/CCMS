-------------------------------------------------------------------------
-- PKG_PLATFORMS
-------------------------------------------------------------------------
-- Todo: Add f_is_valid_decision here and validate proceeding decision in pkg_case_proceedings
CREATE OR REPLACE PACKAGE pkg_proceeding_decisions IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_proceeding_decisions (
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_get_proceeding_decision_details (
        pi_proceeding_decision_id   IN proceeding_decisions.proceeding_decision_id%type,
        po_cursor                   OUT sys_refcursor
    );
    
END pkg_proceeding_decisions;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_proceeding_decisions IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_proceeding_decisions (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                proceeding_decision_id,
                proceeding_decision_name,
                has_next_hearing_date,
                has_order_attachment
            from
                proceeding_decisions
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_proceeding_decisions' || chr(10) || sqlerrm);
    END p_get_all_proceeding_decisions;
-------------------------------------------------------------------------
    PROCEDURE p_get_proceeding_decision_details (
        pi_proceeding_decision_id   IN proceeding_decisions.proceeding_decision_id%type,
        po_cursor                   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                proceeding_decision_id,
                proceeding_decision_name,
                has_next_hearing_date,
                has_order_attachment
            from
                proceeding_decisions
            where
                proceeding_decision_id = pi_proceeding_decision_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_proceeding_decision_details - pi_proceeding_decision_id: ' || pi_proceeding_decision_id
                || chr(10) || sqlerrm);
    END p_get_proceeding_decision_details;
-------------------------------------------------------------------------
END pkg_proceeding_decisions;
/
-------------------------------------------------------------------------