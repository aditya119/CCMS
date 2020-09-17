-------------------------------------------------------------------------
-- PKG_INSIGHTS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_insights IS
-------------------------------------------------------------------------
    PROCEDURE p_pending_disposed_cases (
        pi_user_id  IN case_proceedings.assigned_to%type,
        po_cursor   OUT sys_refcursor
    );

END pkg_insights;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_insights IS
-------------------------------------------------------------------------
    PROCEDURE p_pending_disposed_cases (
        pi_user_id  IN case_proceedings.assigned_to%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                nvl(
                    sum(
                        case
                            when pd.proceeding_decision_name in ('FINAL JUDGEMENT')
                            then 1
                            else 0
                        end), 0) disposed_off,
                nvl(
                    sum(
                        case
                            when pd.proceeding_decision_name not in ('FINAL JUDGEMENT')
                            then 1
                            else 0
                        end), 0) pending
            from
                court_cases cc,
                proceeding_decisions pd
            where   cc.case_status = pd.proceeding_decision_id
                and cc.deleted is null
                and pd.deleted is null
                and (0 = pi_user_id
                    or cc.case_id in (
                        select
                            case_id
                        from
                            case_proceedings
                        where   deleted is null
                            and assigned_to = pi_user_id
                    ));
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_pending_disposed_cases - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END p_pending_disposed_cases;
-------------------------------------------------------------------------
END pkg_insights;
/
-------------------------------------------------------------------------